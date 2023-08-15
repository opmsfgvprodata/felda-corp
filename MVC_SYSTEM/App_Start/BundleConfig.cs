using System.Web;
using System.Web.Optimization;

namespace MVC_SYSTEM
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Asset/Scripts/jquery-{version}.js"
                         ));

            bundles.Add(new ScriptBundle("~/bundles/Modernizrjquery").Include(
                        "~/Asset/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Asset/Scripts/jquery.validate*",
                        "~/Scripts/jquery.unobtrusive*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                       "~/Asset/Scripts/bootstrap.js",
                      "~/Asset/Scripts/respond.js",
                      "~/Asset/alert/simply-toast.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/Closejs").Include(
                       "~/Asset/Scripts/myownjs.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Asset/Content/bootstrap.css",
                      "~/Asset/Content/site.css",
                      "~/Asset/Content/leftnav.css",
                      "~/Asset/alert/simply-toast.css"));

            bundles.Add(new StyleBundle("~/Content/logincss").Include(
                      "~/Asset/Content/bootstrap.min.css",
                      "~/Asset/Content/site.css",
                      "~/Asset/Content/leftnav.css"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                "~/Asset/Scripts/jquery-ui-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/customizeforform").Include(
                "~/Asset/Scripts/Customizer.js"));

            bundles.Add(new StyleBundle("~/Content/jqueryui").Include(
                "~/Asset/Content/themes/base/all.css"));

            bundles.Add(new ScriptBundle("~/bundles/DateTimePickerJS").Include(
                       "~/Asset/Scripts/jquery.datetimepicker.js"));

            bundles.Add(new StyleBundle("~/Content/DateTimePickerCSS").Include(
                      "~/Asset/Content/jquery.datetimepicker.css"));

            bundles.Add(new ScriptBundle("~/bundles/MyCustomeJs").Include(
                       "~/Asset/Scripts/moment.min.js",
                       "~/Asset/Scripts/myScript.js",
                       "~/Asset/Scripts/Checking.js"));

            bundles.Add(new ScriptBundle("~/bundles/CheckingJS").Include(
                        "~/Asset/Scripts/Checking.js"));

            bundles.Add(new ScriptBundle("~/bundles/Script-custom-editor").Include(
                       "~/Asset/Scripts/script-custom-editor.js"));

            BundleTable.EnableOptimizations = false;
        }
    }
}
