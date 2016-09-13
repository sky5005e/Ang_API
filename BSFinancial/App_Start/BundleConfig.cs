using System.Web;
using System.Web.Optimization;

namespace BSFinancial
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));


            ////////////////////////////////////////////////////////////////////////
            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/Template/plugins/bootstrap/css/bootstrap.min.css",
                      "~/Content/Template/css/style.css",
                      "~/Content/Template/css/loading-bar.css"));

            bundles.Add(new StyleBundle("~/Content/plugins").Include(
                      "~/Content/Template/plugins/line-icons/line-icons.css",
                      "~/Content/Template/plugins/font-awesome/css/font-awesome.min.css",
                      "~/Content/Template/plugins/flexslider/flexslider.css",
                      "~/Content/Template/plugins/parallax-slider/css/parallax-slider.css"));

            // Include required angular [Add by Jimmy]
            bundles.Add(new ScriptBundle("~/bundles/BSFAngular")
                .Include("~/Scripts/angular.js")
                //.Include("~/Scripts/angular-highlightjs.min.js")
                .Include("~/Scripts/angular-route.min.js")
                .Include("~/Scripts/angular-animate.min.js")
                .Include("~/Scripts/angular-sanitize.min.js")
                .Include("~/Scripts/angular-ui-router.min.js")
                .Include("~/Scripts/angular-local-storage.min.js")
                .Include("~/Scripts/angular-validator.js")
                .Include("~/Scripts/ngBootbox.min.js")
                .Include("~/Scripts/loading-bar.min.js")
                .Include("~/Scripts/ui-load.js")
                .Include("~/Scripts/ui-jq.js")
                .Include("~/Scripts/ui-validate.js")
//                 .Include("~/Scripts/ui-bootstrap-tpis.min.js")
                .Include("~/Content/Template/plugins/angular-bootstrap/ui-bootstrap-0.12.0.min.js")
                .Include("~/Content/Template/plugins/angular-bootstrap/ui-bootstrap-tpls-0.12.0.min.js")
                .Include("~/Content/Template/plugins/angular-ladda/angular-ladda.min.js")
                .Include("~/Content/Template/plugins/angular-toastr/angular-toastr.min.js")
                .Include("~/Content/Template/plugins/angular-dialog/js/ngDialog.min.js")
                .Include("~/Content/Template/plugins/angular-multi-select/angular-multi-select.js")
                );

            bundles.Add(new ScriptBundle("~/bundles/angular-datatables")
                .Include("~/Content/Template/plugins/angular-datatables/angular-datatables.min.js")
                .Include("~/Content/Template/plugins/angular-datatables/angular-datatables.util.js")
                .Include("~/Content/Template/plugins/angular-datatables/angular-datatables.options.js")
                .Include("~/Content/Template/plugins/angular-datatables/angular-datatables.factory.js")
                .Include("~/Content/Template/plugins/angular-datatables/angular-datatables.renderer.js")
                .Include("~/Content/Template/plugins/angular-datatables/angular-datatables.bootstrap.options.js")
                .Include("~/Content/Template/plugins/angular-datatables/angular-datatables.bootstrap.js")
                .Include("~/Content/Template/plugins/angular-datatables/angular-datatables.directive.js")
                );

            // Set EnableOptimizations to false for debugging. For more information,
            // visit http://go.microsoft.com/fwlink/?LinkId=301862
            BundleTable.EnableOptimizations = true;
        }
    }
}