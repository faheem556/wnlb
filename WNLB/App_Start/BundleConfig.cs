﻿using System.Web;
using System.Web.Optimization;

namespace WNLB
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/_config/bundles/jquery").Include(
                        "~/_configResources/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/_config/bundles/jqueryval").Include(
                        "~/_configResources/Scripts/jquery.validate*",
                        "~/_configResources/Scripts/_val_extension.js"));            

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/_config/bundles/modernizr").Include(
                        "~/_configResources/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/_config/bundles/bootstrap").Include(
                      "~/_configResources/Scripts/bootstrap.js",
                      "~/_configResources/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/_config/Content/css").Include(
                      "~/_configResources/Content/bootstrap.css",
                      "~/_configResources/Content/bootstrap-theme.css",
                      "~/_configResources/Content/site.css"));

            //BundleTable.EnableOptimizations = true;
        }
    }
}
