﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WalkingTec.Mvvm.Core;
using Ignite2019.IoT.Orleans.Controllers;
using Ignite2019.IoT.Orleans.ViewModel.BackgroundJobVMs;
using Ignite2019.IoT.Orleans.Model;
using Ignite2019.IoT.Orleans.DataAccess;

namespace Ignite2019.IoT.Orleans.Test
{
    [TestClass]
    public class BackgroundJobControllerTest
    {
        private BackgroundJobController _controller;
        private string _seed;

        public BackgroundJobControllerTest()
        {
            _seed = Guid.NewGuid().ToString();
            _controller = MockController.CreateController<BackgroundJobController>(_seed, "user");
        }

        [TestMethod]
        public void SearchTest()
        {
            PartialViewResult rv = (PartialViewResult)_controller.Index();
            Assert.IsInstanceOfType(rv.Model, typeof(IBasePagedListVM<TopBasePoco, BaseSearcher>));
            string rv2 = _controller.Search(rv.Model as BackgroundJobListVM);
            Assert.IsTrue(rv2.Contains("\"Code\":200"));
        }

        [TestMethod]
        public void CreateTest()
        {
            PartialViewResult rv = (PartialViewResult)_controller.Create();
            Assert.IsInstanceOfType(rv.Model, typeof(BackgroundJobVM));

            BackgroundJobVM vm = rv.Model as BackgroundJobVM;
            BackgroundJob v = new BackgroundJob();
			
            v.DeviceId = "PtbRfKXo";
            v.Period = 7;
            v.ExecutedCount = 95;
            vm.Entity = v;
            _controller.Create(vm);

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data = context.Set<BackgroundJob>().FirstOrDefault();
				
                Assert.AreEqual(data.DeviceId, "PtbRfKXo");
                Assert.AreEqual(data.Period, 7);
                Assert.AreEqual(data.ExecutedCount, 95);
            }

        }

        [TestMethod]
        public void EditTest()
        {
            BackgroundJob v = new BackgroundJob();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
       			
                v.DeviceId = "PtbRfKXo";
                v.Period = 7;
                v.ExecutedCount = 95;
                context.Set<BackgroundJob>().Add(v);
                context.SaveChanges();
            }

            PartialViewResult rv = (PartialViewResult)_controller.Edit(v.ID.ToString());
            Assert.IsInstanceOfType(rv.Model, typeof(BackgroundJobVM));

            BackgroundJobVM vm = rv.Model as BackgroundJobVM;
            v = new BackgroundJob();
            v.ID = vm.Entity.ID;
       		
            v.DeviceId = "useGnvi";
            v.Period = 81;
            v.ExecutedCount = 3;
            vm.Entity = v;
            vm.FC = new Dictionary<string, object>();
			
            vm.FC.Add("Entity.DeviceId", "");
            vm.FC.Add("Entity.Period", "");
            vm.FC.Add("Entity.ExecutedCount", "");
            _controller.Edit(vm);

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data = context.Set<BackgroundJob>().FirstOrDefault();
 				
                Assert.AreEqual(data.DeviceId, "useGnvi");
                Assert.AreEqual(data.Period, 81);
                Assert.AreEqual(data.ExecutedCount, 3);
            }

        }


        [TestMethod]
        public void DeleteTest()
        {
            BackgroundJob v = new BackgroundJob();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
        		
                v.DeviceId = "PtbRfKXo";
                v.Period = 7;
                v.ExecutedCount = 95;
                context.Set<BackgroundJob>().Add(v);
                context.SaveChanges();
            }

            PartialViewResult rv = (PartialViewResult)_controller.Delete(v.ID.ToString());
            Assert.IsInstanceOfType(rv.Model, typeof(BackgroundJobVM));

            BackgroundJobVM vm = rv.Model as BackgroundJobVM;
            v = new BackgroundJob();
            v.ID = vm.Entity.ID;
            vm.Entity = v;
            _controller.Delete(v.ID.ToString(),null);

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                Assert.AreEqual(context.Set<BackgroundJob>().Count(), 0);
            }

        }


        [TestMethod]
        public void DetailsTest()
        {
            BackgroundJob v = new BackgroundJob();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
				
                v.DeviceId = "PtbRfKXo";
                v.Period = 7;
                v.ExecutedCount = 95;
                context.Set<BackgroundJob>().Add(v);
                context.SaveChanges();
            }
            PartialViewResult rv = (PartialViewResult)_controller.Details(v.ID.ToString());
            Assert.IsInstanceOfType(rv.Model, typeof(IBaseCRUDVM<TopBasePoco>));
            Assert.AreEqual(v.ID, (rv.Model as IBaseCRUDVM<TopBasePoco>).Entity.GetID());
        }

        [TestMethod]
        public void BatchDeleteTest()
        {
            BackgroundJob v1 = new BackgroundJob();
            BackgroundJob v2 = new BackgroundJob();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
				
                v1.DeviceId = "PtbRfKXo";
                v1.Period = 7;
                v1.ExecutedCount = 95;
                v2.DeviceId = "useGnvi";
                v2.Period = 81;
                v2.ExecutedCount = 3;
                context.Set<BackgroundJob>().Add(v1);
                context.Set<BackgroundJob>().Add(v2);
                context.SaveChanges();
            }

            PartialViewResult rv = (PartialViewResult)_controller.BatchDelete(new string[] { v1.ID.ToString(), v2.ID.ToString() });
            Assert.IsInstanceOfType(rv.Model, typeof(BackgroundJobBatchVM));

            BackgroundJobBatchVM vm = rv.Model as BackgroundJobBatchVM;
            vm.Ids = new string[] { v1.ID.ToString(), v2.ID.ToString() };
            _controller.DoBatchDelete(vm, null);

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                Assert.AreEqual(context.Set<BackgroundJob>().Count(), 0);
            }
        }


    }
}
