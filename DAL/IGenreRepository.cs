using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public interface IGenreRepository
    {
        Task<List<Genre>> GetAllGenres();
        Task<Genre> GetGenre(string name);
        Task<Genre> GetGenreByID(int id);
        Task AddGenre(Genre b1);
        Task DeleteGenre(Genre b1);
        Task UpdateGenre(int genreID, Genre updatedGenre);
    }
}
