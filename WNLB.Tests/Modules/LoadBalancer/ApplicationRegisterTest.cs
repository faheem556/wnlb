﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WNLB.Modules.LoadBalancer;

namespace WNLB.Tests.Modules.LoadBalancer
{
    [TestClass]
    public class ApplicationRegisterTest
    {
        [TestMethod]
        public void GetAppByPathTest()
        {
            Application app1 = new StaticApplication("App1", "/", null);
            Application app2 = new StaticApplication("App2", "/App2", null);

            ApplicationRegister register = new ApplicationRegister();
            register.AddAppliction(app1);
            register.AddAppliction(app2);

            Assert.IsTrue(register.GetApplicationForPath("/") == app1);
            Assert.IsTrue(register.GetApplicationForPath("/images/pic0.png") == app1);
            Assert.IsTrue(register.GetApplicationForPath("/App2") == app2);
            Assert.IsTrue(register.GetApplicationForPath("/App2/images/pic0.png") == app2);
        }
    }
}
