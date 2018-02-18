/*! knockout-spa (https://github.com/onlyurei/knockout-spa) * Copyright 2015-2017 Cheng Fan * MIT Licensed (https://raw.githubusercontent.com/onlyurei/knockout-spa/master/LICENSE) */
define(['ko','jquery'], function (ko) {

  var initialValues = {};

  var PageDisposer = {
    init: function (page) {
      initialValues = {};
      $.each(page, function (key, value) {
        var _key = '_' + key;
        if (ko.isObservable(value) && !ko.isComputed(value)) { /* non-computed observables */
          initialValues[key] = value();
        } else if (page[_key] && ko.isObservable(page[_key]) && !ko.isComputed(page[_key])) { /* non-computed observables (es5-option4) */
          initialValues[_key] = page[_key]();
        } else if ((value === null) || (value === undefined) || typeof value === "string" || typeof value === "number" || typeof value === "boolean") { /* primitives */
          initialValues[key] = value;
        }
      });
    },
    dispose: function (page) {
      $.each(initialValues, function (key, value) {
        if (page.hasOwnProperty(key)) {
          if (typeof page[key] === "function") {
            page[key](value);
          } else {
            page[key] = value;
          }
        }
      });
      initialValues = {};
    }
  };

  return PageDisposer;

});
