using System.Web;
using System.Web.Optimization;

namespace WebTraceMonitor.App_Start
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-1.9.1.js",
                        "~/Scripts/jquery.event.drag.js",
                        "~/Scripts/jquery.signalR-1.0.1.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        "~/Scripts/jquery-ui-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.unobtrusive*",
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/webtracemonitor").Include(
                        "~/SlickGrid/slick.core.js",
                        "~/SlickGrid/slick.grid.js",
                        "~/SlickGrid/slick.formatters.js",
                        "~/SlickGrid/slick.editors.js",
                        "~/SlickGrid/slick.dataview.js",
                        "~/SlickGrid/plugins/slick.rowselectionmodel.js",
                        "~/SlickGrid/controls/slick.columnpicker.js",
                        "~/SlickGrid/slick.core.js",
                        "~/Scripts/knockout-2.2.0.js",
                        "~/Scripts/jquery.multiselect.js",
                        "~/Scripts/TestDataGenerator.js",
                        "~/Scripts/DetailsDialog.js",
                        "~/Scripts/AboutDialog.js",
                        "~/Scripts/WebTraceMonitor.js",
                        "~/Scripts/ConnectionManager.js",
                        "~/Scripts/Toolbar.js",
                        "~/Scripts/Main.js"));
        

            bundles.Add(new StyleBundle("~/Content/css").Include(
                        "~/Content/Site.css", 
                        "~/SlickGrid/slick.grid.css",
                        "~/SlickGrid/controls/slick.columnpicker.css"
                        ));

            bundles.Add(new StyleBundle("~/Content/themes/base/css").Include(
                        "~/Content/themes/base/jquery.ui.core.css",
                        "~/Content/themes/base/jquery.ui.resizable.css",
                        "~/Content/themes/base/jquery.ui.selectable.css",
                        "~/Content/themes/base/jquery.ui.accordion.css",
                        "~/Content/themes/base/jquery.ui.autocomplete.css",
                        "~/Content/themes/base/jquery.ui.button.css",
                        "~/Content/themes/base/jquery.ui.dialog.css",
                        "~/Content/themes/base/jquery.ui.slider.css",
                        "~/Content/themes/base/jquery.ui.tabs.css",
                        "~/Content/themes/base/jquery.ui.datepicker.css",
                        "~/Content/themes/base/jquery.ui.progressbar.css",
                        "~/Content/themes/base/jquery-ui-1.10.2.custom.css",
                        "~/Content/jquery.multiselect.css"
                        ));
        }
    }
}