/*! knockout-spa (https://github.com/onlyurei/knockout-spa) * Copyright 2015-2017 Cheng Fan * MIT Licensed (https://raw.githubusercontent.com/onlyurei/knockout-spa/master/LICENSE) */
define(['page-disposer', 'ko', 'director', 'validation'], function (PageDisposer, ko, router) {

    var initialRun = true;

    function hasPermission(pageName) {
        switch (pageName) {
            case "summary":
                return Page.permissions() && Page.permissions().AllowToViewOrder;
            case "configuremeal":
                return Page.permissions() && Page.permissions().AllowToChangeOrder;
            case "meal":
                return Page.permissions() && Page.permissions().AllowToModifyMeal;
            case "report":
                return Page.permissions() && Page.permissions().AllowToViewSales;
            case "manageaccount":
                return Page.permissions() && Page.permissions().AllowToModifyAccount;
            case "managepaymentmethod":
                return Page.permissions() && Page.permissions().AllowToModifyPaymentMethod;
            default:
                return true;
                break;
        }

        return true;
    }

    var Page = {
        init: function (name, data, path, controller) {
            this.loading = false;

            if (!hasPermission(name)) {
                Page.goTo("/home");
                return;
            }

            name = name.toLowerCase();

            if ((this.page().name == name) && (this.page().data == data)) { // if the requested page is the same page, immediately call controller without going further
                if (controller) {
                    controller(data);
                }

                document.title = this.title();

                this.initExtra && this.initExtra.apply(this, Array.prototype.slice.call(arguments, 0));

                return data;
            }

            var autoDispose = this.page().data.dispose && this.page().data.dispose(this);
            this.disposeExtra && this.disposeExtra(this.page().name, this.page().data, this.page().path);
            if (!initialRun && (autoDispose !== false)) { // if not initial run and the requested page is not the same page, dispose current page first before swap to the new page
                // auto-dispose page's exposed observables and primitive properties to initial values. if not desired, return
                // false in dispose function to suppress auto-disposal for all public properties of the page, or make the
                // particular properties private
                PageDisposer.dispose(this.page().data);
            }

            PageDisposer.init(data); //store initial observable and primitive properties values of the page
            var initialized = data.init && data.init(this); // init view model and call controller (optional) before template is swapped-in
            if (initialized === false) {
                return false; // stop initialization if page's init function return false (access control, etc.)
            }
            if (controller) {
                controller(data);
            }

            this.pageClass = [name, ('ontouchstart' in document.documentElement) ? 'touch' : 'no-touch'].join(' ');
            this.page({
                name: name,
                data: data,
                path: path
            }); // to test if template finished rendering, use afterRender binding in the template binding

            document.title = this.title();

            this.initExtra && this.initExtra.apply(this, Array.prototype.slice.call(arguments, 0));

            if (initialRun) {
                ko.validation.init({
                    grouping: { deep: false, observable: false },
                    decorateElement: true,
                    insertMessages: true,
                    decorateElementOnModified: true,
                    decorateInputElement: true,
                    errorClass: 'error-msg',
                    errorMessageClass: 'error-msg',
                    errorElementClass: 'error',
                    //messageTemplate: "errorMessageTemplate"
                });

                ko.applyBindings(this, document.getElementsByTagName('html')[0]); // apply binding at root node to be able to bind to anywhere
                initialRun = false;
            }

            return data;
        },
        page: ko.observable({
            name: '', // name of the page - auto-set by the framework, no need to worry
            data: {} // init, afterRender, controllers, dispose
        }),
        pageClass: '',
        loading: false,
        title: function () {
            //return this.page.name.titleize(); // override in RootBindings as needed
            return "OrderCenter";
        },
        permissions: ko.observable()
    };

    Page.goTo = function (url) {
        Router().setRoute(url);
        if (!!url && url == '/home') {
            var topbar = document.getElementById("topbar");
            topbar.className = 'top-bar row';
        }
        if (!!url && url == '/') {
            var topbar = document.getElementById("topbar");
            topbar.className = 'top-bar row display-none';
        }
    }

    Page.logout = function () {
        require(["utility"], function (utility) {
            utility.baseAjax({
                url: '/Home/Logout',
                type: "POST",
                dataType: "json"
            }).done(function () {
                Page.goTo("/");
            });
        })
    };
    Page.afterRender = function () {
        if (typeof Page.page().data.afterRender === 'function') {
            Page.page().data.afterRender();
        }
    }
    return Page;
});
