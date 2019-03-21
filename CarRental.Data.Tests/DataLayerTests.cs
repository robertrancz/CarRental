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
        public void test_repository_factory_usage()
        {
            var repositoryFactoryTest = new RepositoryFactoryTestClass();
            IEnumerable<Car> cars = repositoryFactoryTest.GetCars();
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

        [TestMethod]
        public void test_factory_mocking1()
        {
            List<Car> cars = new List<Car>()
            {
                new Car() { CarId = 1, Description = "Mustang" },
                new Car() { CarId = 2, Description = "Corvette" }
            };

            Mock<IDataRepositoryFactory> mockDataRepository = new Mock<IDataRepositoryFactory>();
            mockDataRepository.Setup(obj => obj.GetDataRepository<ICarRepository>().Get()).Returns(cars);

            RepositoryFactoryTestClass factoryTest = new RepositoryFactoryTestClass(mockDataRepository.Object);

            IEnumerable<Car> ret = factoryTest.GetCars();

            Assert.IsTrue(ret == cars);
        }

        [TestMethod]
        public void test_factory_mocking2()
        {
            List<Car> cars = new List<Car>()
            {
                new Car() { CarId = 1, Description = "Mustang" },
                new Car() { CarId = 2, Description = "Corvette" }
            };

            Mock<ICarRepository> mockCarRepository = new Mock<ICarRepository>();
            mockCarRepository.Setup(obj => obj.Get()).Returns(cars);

            Mock<IDataRepositoryFactory> mockDataRepository = new Mock<IDataRepositoryFactory>();
            mockDataRepository.Setup(obj => obj.GetDataRepository<ICarRepository>()).Returns(mockCarRepository.Object);

            RepositoryFactoryTestClass factoryTest = new RepositoryFactoryTestClass(mockDataRepository.Object);

            IEnumerable<Car> ret = factoryTest.GetCars();

            Assert.IsTrue(ret == cars);
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

    public class RepositoryFactoryTestClass
    {
        [Import]
        private IDataRepositoryFactory _dataRepositoryFactory;

        public RepositoryFactoryTestClass()
        {
            ObjectBase.Container.SatisfyImportsOnce(this);
        }

        public RepositoryFactoryTestClass(IDataRepositoryFactory dataRepositoryFactory)
        {
            _dataRepositoryFactory = dataRepositoryFactory;
        }

        public IEnumerable<Car> GetCars()
        {
            ICarRepository carRepository = _dataRepositoryFactory.GetDataRepository<ICarRepository>();

            IEnumerable<Car> cars = carRepository.Get();

            return cars;
        }
    }
}
