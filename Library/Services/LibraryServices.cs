using Library.Data;
using Library.ViewModels;
using Library.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Library.Services
{
    public class LibraryServices
    {
        private readonly LibraryDbContext _db;
        private readonly EmailServices _emailService;
        public LibraryServices(LibraryDbContext db, EmailServices emailServices)
        {
            _db = db;
            _emailService = emailServices;
        }

        private async Task<bool> SaveAsync()
        {
            try
            {
                var rowsAffected = await _db.SaveChangesAsync();

                if (rowsAffected > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<GenreBaseModel>> GetAllGenre()
        {
            try
            {
                var GenreList = new List<GenreBaseModel>();

                GenreList = await _db.Genre.ToListAsync();
                return GenreList;
            }
            catch
            {
                return new List<GenreBaseModel>();
            }
        }
        public async Task<BookListViewModel> GetAllBooks()
        {
            try
            {
                var bookList = new BookListViewModel();

                bookList.Books = await _db.Books
                    .Include(i => i.Genre)
                    .ToListAsync();
                return bookList;
            }
            catch
            {
                return new BookListViewModel() { Books = new List<BookBaseModel>() };
            }
        }


        public async Task<BookListViewModel> GetAllAvaibleBooks()
        {
            try
            {
                var bookList = new BookListViewModel();
                bookList.Books = await _db.Books
                    .Include(i => i.Genre)
                    .Where(b => b.Available)
                    .ToListAsync();
                return bookList;
            }
            catch
            {
                return new BookListViewModel() { Books = new List<BookBaseModel>() };
            }
        }


        public async Task<bool> AddBook(BookAddModel model)
        {
            var fileName = model.File.FileName;
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "upload", "images", fileName);

            await using (var stream = new FileStream(path, FileMode.Create))
            {
                await model.File.CopyToAsync(stream);
            }
            var webPath = Path.Combine("upload", "images", fileName);

            var book = new BookBaseModel()
            {
                Id = Guid.NewGuid(),
                Title = model.Title,
                Author = model.Author,
                IdGenre = model.IdGenre,
                Img = webPath,
                Available = model.Available,
            };
            _db.Books.Add(book);
            return await SaveAsync();
        }

        public async Task<BookBaseModel?> GetEditModel(Guid id)
        {
            var book = await _db.Books.FindAsync(id);
            if (book == null)
            {
                return null;
            }
            return book;
        }

        public async Task<bool> EditSave(BookEditModel model)
        {
            var book = await _db.Books.FindAsync(model.Id);
            if (book == null)
            {
                return false;
            }
            book.Title = model.Title;
            book.Author = model.Author;
            book.IdGenre = model.IdGenre;
            book.Img = model.Img;
            book.Available = model.Available;

            return await SaveAsync();
        }

        public async Task<bool> Delete(Guid Id)
        {
            var book = await _db.Books.FindAsync(Id);
            if (book == null)
            {
                return false;
            }
            _db.Books.Remove(book);
            return await SaveAsync();
        }

        private async Task<UserBaseModel> FindOrCreateUser(string name, string surname, string email)
        {
            var user = await _db.User.FirstOrDefaultAsync(u => u.Name == name && u.Surname == surname && u.Email == email);
            if (user == null)
            {
                user = new UserBaseModel
                {
                    IdUser = Guid.NewGuid(),
                    Email = email,
                    Name = name,
                    Surname = surname,
                };
                _db.User.Add(user);
                await SaveAsync();
            }
            return user;
        }


        public async Task<bool> MakeALoan(string nome, string cognome, string email, List<Guid> bookList)
        {
            if (string.IsNullOrEmpty(nome) || string.IsNullOrEmpty(cognome))
            {
                throw new Exception("Nome o cognome non validi");
            }

            var user = await FindOrCreateUser(nome, cognome, email);
            var loan = new Loan
            {
                Id = Guid.NewGuid(),
                IdUser = user.IdUser,
                LoanDate = DateTime.UtcNow,
                LoanBooks = bookList.Select(IdBook => new LoanBooks
                {
                    IdBook = IdBook,
                    IsReturned = false,
                }).ToList(),
            };

            _db.Prestiti.Add(loan);

            var booksToUpdate = _db.Books.Where(b => bookList.Contains(b.Id)).ToList();
            foreach (var book in booksToUpdate)
            {
                book.Available = false;
            }

            try
            {
                await _emailService.SendConfirm(user.Name, user.Email);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Errore durante il salvataggio del prestito.", ex);
            }

            return await SaveAsync();
        }

        public async Task<bool> CancelLoan(Guid loanId)
        {
            var loan = await _db.Prestiti
                .Include(l => l.LoanBooks)
                .FirstOrDefaultAsync(l => l.Id == loanId);

            if (loan == null)
            {
                return false;
            }

            var booksToUpdate = _db.Books.Where(b => loan.LoanBooks.Select(lb => lb.IdBook).Contains(b.Id)).ToList();
            foreach (var book in booksToUpdate)
            {
                book.Available = true;
            }

            _db.Prestiti.Remove(loan);

            try
            {
                return await SaveAsync();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Errore durante la rimozione della prenotazione.", ex);
            }
        }




        public async Task<List<Loan>> GetLoans()
        {
            try
            {
                var loanList = new List<Loan>();

                loanList = await _db.Prestiti
                    .Include(i => i.User)
                    .Include(l => l.LoanBooks)
                    .ThenInclude(lb => lb.Book)
                    .ToListAsync();

                return loanList;
            }
            catch
            {
                return new List<Loan>() { };
            }
        }

    }
}
