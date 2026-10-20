using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using lab7v13;

namespace lab7v13.Tests
{
    public class InMemoryRepositoryTests
    {
        private InMemoryRepository<City> CreateRepository()
        {
            var repo = new InMemoryRepository<City>();
            repo.Add(new City(1, "Київ", "Україна", 2950000));
            repo.Add(new City(2, "Одеса", "Україна", 1010000));
            return repo;
        }

        [Fact]
        public void Add_ShouldAddNewItem_WhenItemIsValid()
        {
            var repo = new InMemoryRepository<City>();
            var city = new City(1, "Рівне", "Україна", 245000);

            repo.Add(city);

            var result = repo.GetById(1);
            Assert.NotNull(result);
            Assert.Equal("Рівне", result.Name);
        }

        [Fact]
        public void Add_ShouldThrowException_WhenDuplicateId()
        {
            var repo = CreateRepository();
            var duplicateCity = new City(1, "Харків", "Україна", 1400000);

            Assert.Throws<InvalidOperationException>(() => repo.Add(duplicateCity));
        }

        [Fact]
        public void GetById_ShouldReturnCorrectItem_WhenIdExists()
        {
            var repo = CreateRepository();

            var city = repo.GetById(1);

            Assert.NotNull(city);
            Assert.Equal("Київ", city.Name);
        }

        [Fact]
        public void GetById_ShouldReturnNull_WhenIdDoesNotExist()
        {
            var repo = CreateRepository();

            var city = repo.GetById(999);

            Assert.Null(city);
        }

        [Fact]
        public void GetAll_ShouldReturnAllItems()
        {
            var repo = CreateRepository();

            var items = repo.GetAll();

            Assert.Equal(2, items.Count());
        }

        [Fact]
        public void Update_ShouldModifyExistingItem()
        {
            var repo = CreateRepository();
            var updatedCity = new City(1, "Київ", "Україна", 3000000);

            repo.Update(updatedCity);

            var cityInRepo = repo.GetById(1);
            Assert.Equal(3000000, cityInRepo.Population);
        }

        [Fact]
        public void Update_ShouldThrowKeyNotFoundException_WhenItemDoesNotExist()
        {
            var repo = CreateRepository();
            var nonExistentCity = new City(999, "Дніпро", "Україна", 990000);

            Assert.Throws<KeyNotFoundException>(() => repo.Update(nonExistentCity));
        }

        [Fact]
        public void Delete_ShouldRemoveItem_WhenIdExists()
        {
            var repo = CreateRepository();

            repo.Delete(1);

            Assert.Null(repo.GetById(1));
            Assert.Single(repo.GetAll());
        }

        [Fact]
        public void Delete_ShouldThrowKeyNotFoundException_WhenIdDoesNotExist()
        {
            var repo = CreateRepository();

            Assert.Throws<KeyNotFoundException>(() => repo.Delete(999));
        }
    }
}