using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using CarRental.Business.Bootstrapper;
using CarRental.Business.Entities;
using CarRental.Data.Contracts;
using Core.Common.Contracts;
using Core.Common.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace CarRental.Data.Tests
{
    [TestClass]
    public class DataLayerTests
    {
        [TestInitialize]
        public void Initialize()
        {
            ObjectBase.Container = MEFLoader.Init();
        }

        [TestMethod]
        public void test_repository_usage()
        {
            var repositoryTest = new RepositoryTestClass();
            IEnumerable<Car> cars = repositoryTest.GetCars();
            Assert.IsNotNull(cars);
        }

        [TestMethod]
        public void test_repository_mocking()
        {
            var expectedCars = new List<Car>
            {
                new Car() {CarId = 1, Description = "Mustang"},
                new Car() {CarId = 2, Description = "Corvette"}
            };

            var mockCarRepository = new Mock<ICarRepository>();
            mockCarRepository.Setup(obj => obj.Get()).Returns(expectedCars);

            var repositoryTest = new RepositoryTestClass(mockCarRepository.Object);

            // Act
            IEnumerable<Car> actualCars = repositoryTest.GetCars();

            Assert.AreEqual(expectedCars, actualCars);
        }
    }

    public class RepositoryTestClass
    {
        [Import]
        private ICarRepository _carRepository;

        public RepositoryTestClass()
        {
            ObjectBase.Container.SatisfyImportsOnce(this);
        }

        public RepositoryTestClass(ICarRepository carRepository)
        {
            _carRepository = carRepository;
        }

        public IEnumerable<Car> GetCars()
        {
            IEnumerable<Car> cars = _carRepository.Get();
            return cars;
        }
    }
}
