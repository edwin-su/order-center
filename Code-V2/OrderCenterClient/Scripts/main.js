var paths = {
    'app': 'Scripts/framework/page',
    'appRouter': 'Scripts/framework/router',
    'routes': 'Scripts/app/shared/routes',
    'director': 'Scripts/lib/director',
    'jquery': 'Scripts/lib/jquery-3.3.1.min',
    'page-disposer': 'Scripts/framework/page-disposer',
    'ko': 'Scripts/lib-ext/knockout-extended',
    'mapping':'Scripts/lib-ext/knockout-mapping',
    'knockout': 'Scripts/lib/knockout-3.4.2',
    'knockout-amd-helpers': 'Scripts/lib/knockout-amd-helpers',
    'validation':'Scripts/lib/knockout.validation.debug',
    'text': 'Scripts/lib/require-text',
    'R': 'Scripts/lib/ramda',
    'chart': 'Scripts/lib/Chart.bundle.min',
    'utils': 'Scripts/lib/utils',

    'utility': 'Scripts/utility/utility',

    'home-js': 'Scripts/app/home/home',
    'home-html': 'templates/home/home.html',
    'login-js': 'Scripts/app/home/login',
    'login-html': 'templates/home/login.html',
    'manageaccount-js': 'Scripts/app/account/manageaccount',
    'manageaccount-html': 'templates/account/manageaccount.html',
    'managepaymentmethod-js': 'Scripts/app/payment/managepaymentmethod',
    'managepaymentmethod-html': 'templates/payment/managepaymentmethod.html',
    'addorder-js': 'Scripts/app/order/addorder',
    'addorder-html': 'templates/order/addorder.html',
    'meal-js': 'Scripts/app/meal/meal',
    'meal-html': 'templates/meal/meal.html',
    'addmeal-js': 'Scripts/app/meal/addmeal',
    'addmeal-html': 'templates/meal/addmeal.html',
    'configuremeal-js': 'Scripts/app/meal/configuremeal',
    'configuremeal-html': 'templates/meal/configuremeal.html',
    'summary-js': 'Scripts/app/report/summary',
    'summary-html': 'templates/report/summary.html',
    'report-js': 'Scripts/app/report/report',
    'report-html': 'templates/report/report.html',
};
requirejs.config({
    baseUrl: '/',
    paths: paths ,
    shim: {
        director: {
            exports: 'Router'
        }
    },
    deps: ['knockout', 'mapping'],
    callback: function (ko, mapping) {
        ko.mapping = mapping;
    }
});

// Start loading the main app file. Put all of
// your application logic in there.
require(['appRouter']);