﻿using Library.Data;
using Library.Models;
using Library.Services;
using Library.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Library.Controllers
{
    public class LibraryController : Controller
    {
        private readonly LibraryServices _libraryServices;
        public LibraryController(LibraryServices libraryServices)
        {
            _libraryServices = libraryServices;
        }

        public async Task<IActionResult> Index()
        {
            var bookList = await _libraryServices.GetAllBooks();
            return View(bookList);
        }

        public async Task<IActionResult> Add()
        {
            var GenreList = await _libraryServices.GetAllGenre();
            if (GenreList.Count > 0)
            {
                ViewBag.GenreList = GenreList;
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddSave(BookAddModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Error while saving entity to database";
            }

            var result = await _libraryServices.AddBook(model);
            if (!result)
            {
                TempData["Error"] = "Errore durante l'aggiunta del libro al databse";
            }

            return RedirectToAction("Index");
        }


        public async Task<IActionResult> Edit(Guid id)
        {
            var book = await _libraryServices.GetEditModel(id);
            if (book == null)
            {
                TempData["Error"] = "Errore nel recuperare i dati del libro selezionato";
            }

            var bookModel = new BookEditModel()
            {
                Id = id,
                Title = book.Title,
                Author = book.Author,
                IdGenre = book.IdGenre,
                Img = book.Img,
                Available = book.Available,
            };

            var GenreList = await _libraryServices.GetAllGenre();
            if (GenreList.Count > 0)
            {
                ViewBag.GenreList = GenreList;
            }

            return View(bookModel);
        }


        [HttpPost("Epibrary/Book/Edit/{id:guid}")]
        public async Task<IActionResult> SaveEdit(Guid id, BookEditModel model)
        {
            var result = await _libraryServices.EditSave(model);
            if (!result)
            {
                TempData["Error"] = "Errore nel salvataggio delle modifiche";
            }

            return RedirectToAction("Index");
        }


        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _libraryServices.Delete(id);
            if (!result)
            {
                TempData["Error"] = "Errore nell'eliminazione del libro dal database";
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Prenota()
        {
            var libriTrovati = await _libraryServices.GetAllAvaibleBooks();
            ViewBag.LibriTrovati = libriTrovati;
            var viewModel = new LoanViewModel
            {
                SelectedBooks = new List<Guid>() // Inizializza la lista dei libri selezionati
            };
            return View(viewModel);
        }

        // Azione per confermare la prenotazione
        [HttpPost]
        public async Task<IActionResult> Prenota(LoanViewModel prenotazione)
        {
            if (ModelState.IsValid)
            {
                await _libraryServices.MakeALoan(prenotazione.Name, prenotazione.Surname, prenotazione.Email, prenotazione.SelectedBooks);
                return RedirectToAction("Index");
            }
            else
            {
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors);
                    foreach (var error in errors)
                    {
                        Console.WriteLine(error.ErrorMessage); // O usa un logger
                    }
                    return View(prenotazione);
                }
            }
        }

        public async Task<IActionResult> CancelLoan(Guid loanId)
        {
            try
            {
                bool success = await _libraryServices.CancelLoan(loanId);
                if (success)
                    return RedirectToAction("Prenotazioni");

                return BadRequest(new { message = "Impossibile annullare la prenotazione." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }



        public async Task<IActionResult> Prenotazioni()
        {
            var prenotazioni = await _libraryServices.GetLoans();
            return View(prenotazioni);
        }
    }

}

