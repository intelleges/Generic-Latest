using System.Web;
using System.Web.Optimization;

namespace Generic
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                     "~/Scripts/jquery-1.9.1.js", "~/Scripts/jquery-migrate-1.2.1.js", "~/Scripts/alertify.js", "~/Scripts/jquery.blockUI.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        "~/Scripts/jquery-ui-1.9.2.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.unobtrusive*",
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new StyleBundle("~/Content/css").Include("~/Content/site.css"));

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
"~/Content/themes/base/alertify.core.css",
"~/Content/themes/base/alertify.default.css",
    

                        "~/Content/themes/base/jquery.ui.theme.css"));
            bundles.Add(new StyleBundle("~/Content/bootstrap").Include("~/Content/bootstrap.css"));
            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include("~/Scripts/bootstrap.js", "~/Scripts/jquery.validate.js", "~/Scripts/validator.js", "~/Scripts/alertify.js"));
            bundles.Add(new StyleBundle("~/Content/jstree").Include("~/Scripts/jsTree3/themes/default/style.css"));

            bundles.Add(new ScriptBundle("~/bundles/jstree").Include("~/Scripts/jsTree3/jstree.js"));


            bundles.Add(new ScriptBundle("~/bundles/registrationJs").Include(
                   "~/Contents/RegistrationStyle/js/registration.js"
                   ));
            bundles.Add(new StyleBundle("~/bundles/registrationCss").Include(
                   "~/Contents/RegistrationStyle/css/registration.css"
                   ));
            bundles.Add(new StyleBundle("~/bundles/alertifyCss").Include(
                  "~/Content/themes/base/alertify.core.css",
                   "~/Content/themes/base/alertify.default.css"
                   ));
            bundles.Add(new StyleBundle("~/bundles/progressBarCss").Include(
                 "~/Content/ProgressBar/skins/tiny-green/progressbar.css",
                  "~/Content/ProgressBar/skins/default/progressbar.css"
                  ));

            bundles.Add(new ScriptBundle("~/bundles/animatedCollapseJs").Include(
                  "~/contents/js/animatedcollapse.js"
                   ));

            bundles.Add(new StyleBundle("~/bundles/HomebootstrapCss").Include("~/Contents/AdminStyle/css/bootstrap.min.css"));
            bundles.Add(new StyleBundle("~/bundles/HomeStyles").Include("~/Contents/AdminStyle/css/styleHome.css"));
            bundles.Add(new StyleBundle("~/bundles/Font-awesome").Include("~/font-awesome/css/font-awesome.min.css"));

            bundles.Add(new ScriptBundle("~/bundles/Homejquery").Include("~/Contents/AdminStyle/js/jquery.js"));
            bundles.Add(new ScriptBundle("~/bundles/HomeBootstrapJs").Include("~/Contents/AdminStyle/js/bootstrap.min.js"));
        }
    }
}