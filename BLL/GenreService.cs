using Common;
using DAL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    class GenreService : IGenereService
    {
        IGenreRepository _genreRep;
        IGeneralRepository _generalRep;
        public GenreService(IGenreRepository genreRep, IGeneralRepository generalRep)
        {
            _generalRep = generalRep;
            _genreRep = genreRep;
        }
        public async Task AddNewGenre(string name)
        {
            if (await CheckGenre(name) == false)
            {
                try
                {
                    Genre g1 = new Genre(name);
                    await _genreRep.AddGenre(g1);
                }
                catch (Exception e)
                {
                    if (e is GenreException
                         || e is DALException)
                    {
                        await GeneralLibraryLogic.SaveToLogFile(e.ToString());
                        throw new BLLGenreException("Cannot add a new genre atm try again later or call a manager");
                    }
                    else
                    {
                        await GeneralLibraryLogic.SaveToLogFile(e.ToString());
                        throw new LibraryException("Unknown error inform a manager!");
                    }
                }
            }
            else
            {
                await GeneralLibraryLogic.SaveToLogFile("Genre Already Exist");
                throw new LibraryException("Genre already exist");
            }
        }
        /// <summary>
        /// Check if a genre name already exist.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<bool> CheckGenre(string name)
        {
            try
            {
                Genre gen = await _genreRep.GetGenre(name);
                if (gen.Name == null)
                    return false;
                return true;
            }
            catch (Exception e)
            {
                if (e is GenreException
                     || e is DALException)
                {
                    await GeneralLibraryLogic.SaveToLogFile(e.ToString());
                    throw new BLLGenreException("Cannot check a genre atm try again later or call a manager");
                }
                else
                {
                    await GeneralLibraryLogic.SaveToLogFile(e.ToString());
                    throw new LibraryException("Unknown error inform a manager!");
                }
            }

        }

        public async Task DeleteGenre(string name)
        {
            try
            {
                Genre g1 = await GetGenre(name);
                if (g1.Name != null)
                    await _genreRep.DeleteGenre(g1);
            }
            catch (Exception e)
            {
                if (e is GenreException
                     || e is DALException)
                {
                    await GeneralLibraryLogic.SaveToLogFile(e.ToString());
                    throw new BLLGenreException("Cannot delete a genre atm try again later or call a manager");
                }
                else
                {
                    await GeneralLibraryLogic.SaveToLogFile(e.ToString());
                    throw new LibraryException("Unknown error inform a manager!");
                }
            }
        }

        public async Task<List<string>> GetAllGenreNames()
        {
            List<string> genres = new List<string>();
            try
            {
                List<Genre> genresL = await _genreRep.GetAllGenres();
                for (int i = 0; i < genresL.Count; i++)
                {
                    genres.Add(genresL[i].Name);
                }
            }
            catch (Exception e)
            {
                if (e is GenreException
                     || e is DALException)
                {
                    await GeneralLibraryLogic.SaveToLogFile(e.ToString());
                    throw new BLLGenreException("Cannot get all genres atm try again later or call a manager");
                }
                else
                {
                    await GeneralLibraryLogic.SaveToLogFile(e.ToString());
                    throw new LibraryException("Unknown error inform a manager!");
                }
            }
            return genres;
        }
        /// <summary>
        /// Get a genre by its name
        /// </summary>
        /// <param name="name"></param>
        /// <returns>Genre found or an empty genre if not found</returns>
        public async Task<Genre> GetGenre(string name)
        {
            try
            {
                return await _genreRep.GetGenre(name);
            }
            catch (Exception e)
            {
                if (e is GenreException
                     || e is DALException)
                {
                    await GeneralLibraryLogic.SaveToLogFile(e.ToString());
                    throw new BLLGenreException("Cannot get a genre atm try again later or call a manager");
                }
                else
                {
                    await GeneralLibraryLogic.SaveToLogFile(e.ToString());
                    throw new LibraryException("Unknown error inform a manager!");
                }
            }
        }

        public async Task UpdateGenre(string name, string newName)
        {
            try
            {
                Genre g1 = await GetGenre(name);
                Genre updated = new Genre();
                updated.ID = g1.ID;
                updated.Name = newName;
                await _genreRep.UpdateGenre(updated.ID, updated);
            }
            catch (Exception e)
            {
                if (e is GenreException
                     || e is DALException)
                {
                    await GeneralLibraryLogic.SaveToLogFile(e.ToString());
                    throw new BLLGenreException("Cannot update a genre atm try again later or call a manager");
                }
                else
                {
                    await GeneralLibraryLogic.SaveToLogFile(e.ToString());
                    throw new LibraryException("Unknown error inform a manager!");
                }
            }
        }
    }
}
