using Library.Data;
using Library.ViewModels;
using Library.Models;
using Microsoft.EntityFrameworkCore;

namespace Library.Services
{
    public class LibraryServices
    {
        private readonly LibraryDbContext _db;
        public LibraryServices(LibraryDbContext db)
        {
            _db = db;
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

        public async Task<bool> AddBook(BookAddModel model)
        {
            var book = new BookBaseModel()
            {
                Id = Guid.NewGuid(),
                Title = model.Title,
                Author = model.Author,
                IdGenre = model.IdGenre,
                Img = model.Img,
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



    }
}
