﻿using Application.Exceptions;
using Application.Libreria.Specifications;
using Domain.Libreria;
using Infrastructure.Data;
using Infrastructure.Data.Libreria;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Libreria.Implementations
{
    public class AutorService : IAutorService
    {
        private readonly LibreriaUnitOfWork unitOfWork;

        public AutorService(LibreriaUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task AgregarLibroAsync(int isbn, int autorId)
        {
            CancellationToken cancelationToken = new CancellationToken();

            // Verificar si el autor existe
            var autor = await unitOfWork.AutorRepository.GetEntityAsync(autorId);
            if (autor != null)
                throw new EntityNotFoundException(typeof(Autor));

            // Verificar si el libro existe
            var libro = await unitOfWork.LibroRepository.GetEntityAsync(isbn);
            if (libro == null)
                throw new EntityNotFoundException(typeof(Libro));

            // Verificar si el autor ya está relacionado con este libro
            if (autor.Libros.Contains(libro))
                throw new EntityIsAlreadyRelatedToException(typeof(Autor), typeof(Libro));

            // Relacionar el libro al autor
            autor.Libros.Add(libro);
            unitOfWork.AutorRepository.UpdateEntity(autor);

            // Guardar los cambios en el contexto
            await unitOfWork.CommitAsync(cancelationToken);
        }

        public async Task<Autor> CreateAutorAsync(Autor autor)
        {
            CancellationToken cancelationToken = new CancellationToken();
            var result = await unitOfWork.AutorRepository.InsertEntityAsync(autor);
            await unitOfWork.CommitAsync(cancelationToken);
            return result;
        }

        public async Task DeleteAutorAsync(int id)
        {
            CancellationToken cancelationToken = new CancellationToken();
            await unitOfWork.AutorRepository.DeleteEntityAsync(id);
            await unitOfWork.CommitAsync(cancelationToken);
        }

        public async Task DeleteAutorAsync(Expression<Func<Autor, bool>> cond)
        {
            CancellationToken cancelationToken = new CancellationToken();
            await unitOfWork.AutorRepository.DeleteEntitiesAsync(cond);
            await unitOfWork.CommitAsync(cancelationToken);
        }

        public async Task<IEnumerable<Autor>> GetAllAutoresAsync()
        {
            return await unitOfWork.AutorRepository.GetEntitiesAsync();
        }

        public async Task<Autor> GetAutorAsync(int id)
        {
            return await unitOfWork.AutorRepository.GetEntityAsync(id);
        }

        public async Task<Autor> GetAutorAsync(Expression<Func<Autor, bool>> cond)
        {
            return await unitOfWork.AutorRepository.GetEntityAsync(cond);
        }

        public async Task<IEnumerable<Autor>> GetAutoresAsync(Expression<Func<Autor, bool>> cond)
        {
            return await unitOfWork.AutorRepository.GetEntitiesAsync(cond);
        }

        public async Task RemoverLibroAsync(int isbn, int autorId)
        {
            // Verificar si el autor existe
            var autor = await unitOfWork.AutorRepository.GetEntityAsync(autorId);
            if (autor == null)
                throw new EntityNotFoundException(typeof(Autor));

            // Verificar si el libro existe
            var libro = await unitOfWork.LibroRepository.GetEntityAsync(isbn);
            if (libro == null)
                throw new EntityNotFoundException(typeof(Libro));

            // Verificar que el autor esté relacionado con el libro
            if (autor.Libros.Where(libro => libro.Id == isbn).Any())
                throw new EntityIsNotRelatedToException(typeof(Autor), typeof(Libro));

            // Remover el libro del autor
            autor.Libros.Remove(libro);

            // Guardar los cambios
            CancellationToken cancellationToken = new CancellationToken();
            await unitOfWork.CommitAsync(cancellationToken);
        }

        public async Task UpdateAutorAsync(Autor autor)
        {
            CancellationToken cancelationToken = new CancellationToken();
            unitOfWork.AutorRepository.UpdateEntity(autor);
            await unitOfWork.CommitAsync(cancelationToken);
        }
    }
}