﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CarRental.Business.Entities;
using CarRental.Data.Contracts;
using Core.Common.Data;

namespace CarRental.Data
{
    [Export(typeof(IAccountRepository))]    // For Dependency Injection
    [PartCreationPolicy(CreationPolicy.NonShared)]  // For EF not to use the singleton pattern
    public class AccountRepository : DataRepositoryBase<Account>, IAccountRepository
    {
        protected override Account AddEntity(CarRentalContext entityContext, Account entity)
        {
            return entityContext.AccountSet.Add(entity);
        }

        protected override IEnumerable<Account> GetEntities(CarRentalContext entityContext)
        {
            return from e in entityContext.AccountSet select e;
        }

        protected override Account GetEntity(CarRentalContext entityContext, int id)
        {
            var query = (from e in entityContext.AccountSet
                where e.AccountId == id
                select e);

            var result = query.FirstOrDefault();
            return result;
        }

        protected override Account UpdateEntity(CarRentalContext entityContext, Account entity)
        {
            return (from e in entityContext.AccountSet
                where e.AccountId == entity.AccountId
                select e).FirstOrDefault();
        }

        public Account GetByLogin(string login)
        {
            using (var entityContext = new CarRentalContext())
            {
                return (from e in entityContext.AccountSet
                    where e.LoginEmail == login
                    select e).FirstOrDefault();
            }
        }
    }
}
