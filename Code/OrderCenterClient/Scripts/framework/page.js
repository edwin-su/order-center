/*! knockout-spa (https://github.com/onlyurei/knockout-spa) * Copyright 2015-2017 Cheng Fan * MIT Licensed (https://raw.githubusercontent.com/onlyurei/knockout-spa/master/LICENSE) */
define(['page-disposer', 'ko', 'director'], function (PageDisposer, ko, router) {

    var initialRun = true;

    var Page ={
        init: function (name, data, path, controller) {
            this.loading = false;

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
        }
    };

    Page.goTo = function (url) {
        Router().setRoute(url);
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

    return Page;
});
