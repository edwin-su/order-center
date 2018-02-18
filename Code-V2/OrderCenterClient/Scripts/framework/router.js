/*! knockout-spa (https://github.com/onlyurei/knockout-spa) * Copyright 2015-2017 Cheng Fan * MIT Licensed (https://raw.githubusercontent.com/onlyurei/knockout-spa/master/LICENSE) */
define(['app', 'routes', 'director', 'jquery'], function (
  Page, Routes, Router) {

  function initPage(pageModulePath, controller) {
    require([pageModulePath+'-js'], function (page) {
      var pathParts = pageModulePath.split('/');
      var pageName = pathParts.slice(1, pathParts.length - 1).join('-');
      var initialized = Page.init(pageModulePath, page, pageModulePath, controller);
      if (initialized === false) {
        routes['/error/:code'](403);
      }
    });
  }

  var routes = {};
  $.each(Routes, function (key, value) {
    var values = value.split(' ');
    var pageModulePath = values[0];
    var controllerName = values[1];
    routes[key] = function () {
      Page.loading = true;
      var args = Array.prototype.slice.call(arguments, 0);
      var controller = controllerName ? function (page) {
        page.controllers[controllerName].apply(page, args);
      } : null;
      initPage(pageModulePath, controller);
    };
  });

  var router = new Router(routes).configure({
    strict: false,
    html5history: true, //TODO change to false to use hash if it's not practical to use HTML5 history api (e.g. no catch-all route on the server side, need to support IE 9 and below, etc.)
    convert_hash_in_init: false,
    notfound: function () {
      routes['/error/:code'](404);
    }
  });

  var urlNotAtRoot = window.location.pathname && (window.location.pathname != '/');

  if (!router.historySupport && urlNotAtRoot) {
    window.location.href = '/#' + (window.location.pathname.startsWith('/') ? '' : '/') + window.location.pathname;
    return;
  }

  if (urlNotAtRoot) {
    router.init();
  } else {
    router.init('/');
  }

  $('body').on('click', 'a[href]', function (event) {
      var href = $(this).attr('href');
    if (href && !href.startsWith('#') &&
        ($(this).attr('target') != '_blank') && !$(this).data('go') && !event.ctrlKey && !event.metaKey) {
          event.preventDefault();
          Page.loading = true;
          router.setRoute(href);
    }
  });

  return router;

});
