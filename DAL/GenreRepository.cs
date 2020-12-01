using Common;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class GenreRepository : IGenreRepository
    {

        public GenreRepository( )
        {
        }
        public async Task AddGenre(Genre g1)
        {
            string insertQuery = "INSERT INTO Genres(GenreName)" +
                               $"VALUES(@genreName)";
            try
            {
                SqliteCommand command = new SqliteCommand(insertQuery);
                command.Parameters.AddWithValue("@genreName", g1.Name);
                SqlFileAccess.SqlPerformQuery(command);
            }
            catch (Exception e)
            {
                if (e is SqliteException || e is ArgumentOutOfRangeException)
                {
                    await GeneralRepository.SaveToLogFile(e.ToString());
                    throw new GenreException("Cant add genre");
                }
                else
                {
                    await GeneralRepository.SaveToLogFile(e.ToString());
                    throw new DALException("Unknown Error");
                }
            }
        }

        public async Task DeleteGenre(Genre g1)
        {
            string deleteQuery = $"DELETE FROM Genres WHERE GenreID = @genreID";
            try
            {
                SqliteCommand command = new SqliteCommand(deleteQuery);
                command.Parameters.AddWithValue("@genreID", g1.ID);
                SqlFileAccess.SqlPerformQuery(command);
            }
            catch (Exception e)
            {
                if (e is SqliteException || e is ArgumentOutOfRangeException)
                {
                    await GeneralRepository.SaveToLogFile(e.ToString());
                    throw new GenreException("Cant delete genre");
                }
                else
                {
                    await GeneralRepository.SaveToLogFile(e.ToString());
                    throw new DALException("Unknown Error");
                }
            }
        }
        /// <summary>
        /// Get all genres exist
        /// </summary>
        /// <returns>List of all genres or null if not genres exist</returns>
        public async Task<List<Genre>> GetAllGenres()
        {
            List<Genre> genres = new List<Genre>();
            string GetgenresQuery = "SELECT GenreID,GenreName " +
                                   "FROM Genres ";
            try
            {
                SqliteCommand command = new SqliteCommand(GetgenresQuery);
                List<string> genresString = SqlFileAccess.GetData(command);
                for (int i = 0; i < genresString.Count; i++)
                {
                    genres.Add(TextToGenre(genresString[i]));
                }
                return genres;
            }
            catch (Exception e)
            {
                if (e is SqliteException || e is ArgumentOutOfRangeException)
                {
                    await GeneralRepository.SaveToLogFile(e.ToString());
                    throw new GenreException("Cant get all genres");
                }
                else
                {
                    await GeneralRepository.SaveToLogFile(e.ToString());
                    throw new DALException("Unknown Error");
                }
            }
        }
        /// <summary>
        /// Get genre by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns>returns the genre found or an empty genre if not found</returns>
        public async Task<Genre> GetGenre(string name)
        {
            string getGenreQuery = $"SELECT GenreID,GenreName FROM Genres WHERE GenreName = @name";
            try
            {
                SqliteCommand command = new SqliteCommand(getGenreQuery);
                command.Parameters.AddWithValue("@name", name);
                List<string> genreString = SqlFileAccess.GetData(command);
                if (genreString.Count == 0)
                    return new Genre();
                return TextToGenre(genreString[0]);
            }
            catch (Exception e)
            {
                if (e is SqliteException || e is ArgumentOutOfRangeException)
                {
                    await GeneralRepository.SaveToLogFile(e.ToString());
                    throw new GenreException("Cant get genre by name");
                }
                else
                {
                    await GeneralRepository.SaveToLogFile(e.ToString());
                    throw new DALException("Unknown Error");
                }
            }
        }
        /// <summary>
        /// Get genre by its id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>return the genre found, or an empty genre if not found</returns>
        public async Task<Genre> GetGenreByID(int id)
        {
            string getGenreQuery = $"SELECT GenreID,GenreName FROM Genres WHERE GenreID = @id";
            try
            {
                SqliteCommand command = new SqliteCommand(getGenreQuery);
                command.Parameters.AddWithValue("@id", id);
                List<string> genreString = SqlFileAccess.GetData(command);
                if (genreString.Count == 0)
                    return new Genre();
                return TextToGenre(genreString[0]);
            }
            catch (Exception e)
            {
                if (e is SqliteException || e is ArgumentOutOfRangeException)
                {
                    await GeneralRepository.SaveToLogFile(e.ToString());
                    throw new GenreException("Cant get genre by id");
                }
                else
                {
                    await GeneralRepository.SaveToLogFile(e.ToString());
                    throw new DALException("Unknown Error");
                }
            }
        }

        public async Task UpdateGenre(int genreID, Genre updatedGenre)
        {
            string updateGenre = "UPDATE Genres " +
                                 $"Set GenreName = @genreName"+
                                 $"WHERE GenreID = @genreID";
            try
            {
                SqliteCommand command = new SqliteCommand(updateGenre);
                command.Parameters.AddWithValue("@genreName", updatedGenre.Name);
                command.Parameters.AddWithValue("@genreID", updatedGenre.ID);
                SqlFileAccess.SqlPerformQuery(command);
            }
            catch (Exception e)
            {
                if (e is SqliteException || e is ArgumentOutOfRangeException)
                {
                    await GeneralRepository.SaveToLogFile(e.ToString());
                    throw new GenreException("Cant update genre");
                }
                else
                {
                    await GeneralRepository.SaveToLogFile(e.ToString());
                    throw new DALException("Unknown Error");
                }
            }
        }
        private Genre TextToGenre(string text)
        {
            try
            {
                string[] genreProps = text.Split(',');
                Genre genre = new Genre();
                genre.ID = int.Parse(genreProps[0]);
                genre.Name = genreProps[1];
                return genre;
            }
            catch (Exception e)
            {
                if (e is ArgumentOutOfRangeException)
                {
                    GeneralRepository.SaveToLogFile(e.ToString());
                    throw new GenreException("Cant convert text to genre");
                }
                else
                {
                    GeneralRepository.SaveToLogFile(e.ToString());
                    throw new DALException("Unknown Error");
                }
            }
        }
    }
}
