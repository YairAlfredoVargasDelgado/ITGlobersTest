using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Domain.Libreria;
using ITGlobersTest.Services;

namespace ITGlobersTest.Pages.Editoriales
{
    public class CreateModel : PageModel
    {
        private readonly EditorialService editorialService;

        public CreateModel(EditorialService autorService)
        {
            this.editorialService = autorService;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            //var libros = await libroService.GetLibrosAsync();
            //ViewData["libros"] = libros.ToList();

            return Page();
        }

        [BindProperty]
        public Categoria Editorial { get; set; }
        [BindProperty]
        public int selectedLibro { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            await editorialService.SaveEditorialAsync(Editorial, selectedLibro);

            return RedirectToPage("./Index");
        }
    }
}
