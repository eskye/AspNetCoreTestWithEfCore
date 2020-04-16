using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using CourseManager.API.DbContexts;
using CourseManager.API.Entities;
using CourseManager.API.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CourseManager.API.Test
{
    public class AuthRepositoryTest
    {

        private DbContextOptions<CourseContext> GetContext(Guid dbId) // Enabling Test Isolation
        {
            ///
            ///Test Isolation means write clean and seperate database of each unit test you write
            /// 
//            var options = new DbContextOptionsBuilder<CourseContext>()
//                .UseInMemoryDatabase($"CourseDatabaseTesting-{dbId}")
//                .Options;
            //Using Sqlite

            var sqlConnetionBuilder = new SqliteConnectionStringBuilder {DataSource = $":memory:"};
            var connection = new SqliteConnection(sqlConnetionBuilder.ToString());
            var options = new DbContextOptionsBuilder<CourseContext>().UseSqlite(connection).Options;
            return options;
        }
        [Fact]
        public void GetAuthors_PageSizeIsThree_ReturnsThreeAuthors()
        {
            //Arrange
            var contextOptions = GetContext(Guid.NewGuid());
            using (var context = new CourseContext(contextOptions))
            {
              context.Database.OpenConnection();
              context.Database.EnsureCreated();
                context.Countries.Add(new Country
                {
                    Id = "BE",
                    Description = "Belgium"

                });

                context.Countries.Add(new Country
                {
                    Id = "US",
                    Description = "United States Of America"

                });

                context.Authors.Add(new Author {CountryId = "BE", FirstName = "Sunkanmi", LastName = "Ijatuyi"});
                context.Authors.Add(new Author {CountryId = "US", FirstName = "Temitope", LastName = "Adewale"});
                context.Authors.Add(new Author {CountryId = "BE", FirstName = "OmoTayo", LastName = "Ajomale"});
                context.Authors.Add(new Author {CountryId = "US", FirstName = "Deborah", LastName = "Adedola"});
                context.Authors.Add(new Author {CountryId = "BE", FirstName = "Ayishat", LastName = "Arowolo"});

                context.SaveChanges();
                
            }

            using (var context = new CourseContext(contextOptions))
            {
                var authorRepository = new AuthorRepository(context);

                //Act

                var authors = authorRepository.GetAuthors(1, 3);

                //Assert

                Assert.Equal(3, authors.Count());
            }

        }

        [Fact]
        public void GetAuthor_EmptyGuid_ThrowsArgumentException()
        {
            //Arrange
            var context = GetContext(Guid.NewGuid());
            using (var dbContext = new CourseContext(context))
            {
                dbContext.Database.OpenConnection();
                dbContext.Database.EnsureCreated();
                var authorRepository = new AuthorRepository(dbContext);

                //Assert

                Assert.Throws<ArgumentException>(() =>
                {
                    //Act
                    authorRepository.GetAuthor(Guid.Empty);
                });
            }

        }

        [Fact]
        public void AddAuthor_AuthorWithoutCountryId_AuthorHasBEAsCountryId()
        {
            //Arrange
            var contextOptions = GetContext(Guid.NewGuid()); 
            using (var context = new CourseContext(contextOptions))
            {
                context.Database.OpenConnection();
                context.Database.EnsureCreated();
                context.Countries.Add(new Country
                {
                    Id = "BE",
                    Description = "Belgium"

                });

                context.SaveChanges();
            }

            using (var context = new CourseContext(contextOptions))
            {
                var authorRepository = new AuthorRepository(context);
                var authorToAdd = new Author
                {
                    FirstName = "Sunkanmi",
                    LastName = "Ijatuyi",
                    Id = Guid.Parse("d84d3d7e-3fbc-4956-84a5-5c57c2d86d7b")
                };
                //Act

                authorRepository.AddAuthor(authorToAdd);
                authorRepository.SaveChanges();
            }

            using (var context = new CourseContext(contextOptions))
            {
                //Assert
                var authorRepository = new AuthorRepository(context);
                var addedAuthor = authorRepository.GetAuthor(Guid.Parse("d84d3d7e-3fbc-4956-84a5-5c57c2d86d7b"));
                Assert.Equal("BE", addedAuthor.CountryId);
            }

        }


    }
}
