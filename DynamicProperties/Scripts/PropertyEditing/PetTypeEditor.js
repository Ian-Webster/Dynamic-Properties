function ErrorModel(data) {
    var self = this;
    self.ErrorMessage = data;
}

function PropertyVM(propertyId) {

    var self = this;

    self.PropertyID = propertyId;
    self.PropertyType = ko.observable();
    self.PropertyName = ko.observable();
    self.Required = ko.observable();

    self.template = 'petType-PropertyEditor-template';

    self.SavePropertyChanges = function () {
        self.modal.close(self);
    };

    self.Cancel = function () {
        self.modal.close();
    };
}

function PetTypeViewModel() {

    var self = this;
    self.PetTypeID = $('#PetTypeID').val();
    self.Name = ko.observable($('#Name').val());
    self.Properties = ko.observableArray([]);

    self.PropertyTypes = [
        { Name: "Textbox", Value: 1 },
        { Name: "Boolean", Value: 2 },
        { Name: "Numeric", Value: 3 }
    ];
    self.Errors = ko.observableArray();

    self.ShowPropertyEditor = function (property) {
        if (self.Properties.indexOf(property) < 0) {
            //new property
            property = new PropertyVM(self.Properties().length + 1, 1, '', false);
        }
        showModal({
            viewModel: property,
            context: this // Set context so we don't need to bind the callback function
        }).then(this.SavePropertyChanges);
    }

    self.SavePropertyChanges = function (property) {
        if (property.PropertyID > self.Properties().length) {
            //new element
            self.AddProperty(property);
        }
    }

    self.AddProperty = function (property) {
        self.Properties.push(property);
    }

    self.Save = function () {
        $.ajax(location.href, {
            data: ko.toJSON({ petType: self }),
            type: "post", contentType: "application/json",
            success: function (data) {
                if (data.result) {
                    //saved ok, return to index
                    location.href = 'index';
                }
                else {
                    //failed
                    var mappedErrors = $.map(data.errors, function (item) { return new ErrorModel(item) });
                    self.Errors(mappedErrors);
                }

            },
            fail: function (jqxhr, textStatus, error) {
                //handler in the event of an error
                var err = textStatus + ', ' + error;
                alert("Request Failed: " + err);
            }
        });
    }

    /* inital population of the properties array */
    var propCount = parseInt($('#PropertyCount').val());
    var prop;
    for (var i = 0; i < propCount; i++) {
        prop = new PropertyVM($('#Properties_' + i + '_\\.PropertyID').val());
        prop.PropertyType($('#Properties_' + i + '_\\.PropertyType').val());
        prop.PropertyName($('#Properties_' + i + '_\\.PropertyName').val());
        prop.Required($('#Properties_' + i + '_\\.Required').val());

        self.AddProperty(prop);
    }




}

$(document).ready(function () {
    //apply knockout binding of the pet view model
    ko.applyBindings(new PetTypeViewModel());
});


//helper methods from http://aboutcode.net/2012/11/15/twitter-bootstrap-modals-and-knockoutjs.html

var createModalElement = function (templateName, viewModel) {
    var temporaryDiv = addHiddenDivToBody();
    var deferredElement = $.Deferred();
    ko.renderTemplate(
        templateName,
        viewModel,
        // We need to know when the template has been rendered,
        // so we can get the resulting DOM element.
        // The resolve function receives the element.
        {
            afterRender: function (nodes) {
                // Ignore any #text nodes before and after the modal element.
                var elements = nodes.filter(function (node) {
                    return node.nodeType === 1; // Element
                });
                deferredElement.resolve(elements[0]);
            }
        },
        // The temporary div will get replaced by the rendered template output.
        temporaryDiv,
        "replaceNode"
    );
    // Return the deferred DOM element so callers can wait until it's ready for use.
    return deferredElement;
};

var addHiddenDivToBody = function () {
    var div = document.createElement("div");
    div.style.display = "none";
    document.body.appendChild(div);
    return div;
};

var showModal = function (options) {
    if (typeof options === "undefined") throw new Error("An options argument is required.");
    if (typeof options.viewModel !== "object") throw new Error("options.viewModel is required.");

    var viewModel = options.viewModel;
    var template = options.template || viewModel.template;
    var context = options.context;
    if (!template) throw new Error("options.template or options.viewModel.template is required.");

    return createModalElement(template, viewModel)
        .pipe($) // jQueryify the DOM element
        .pipe(function ($ui) {
            var deferredModalResult = $.Deferred();
            addModalHelperToViewModel(viewModel, deferredModalResult, context);
            showTwitterBootstrapModal($ui);
            whenModalResultCompleteThenHideUI(deferredModalResult, $ui);
            whenUIHiddenThenRemoveUI($ui);
            return deferredModalResult;
        });
};

var addModalHelperToViewModel = function (viewModel, deferredModalResult, context) {
    // Provide a way for the viewModel to close the modal and pass back a result.
    viewModel.modal = {
        close: function (result) {
            if (typeof result !== "undefined") {
                deferredModalResult.resolveWith(context, [result]);
            } else {
                // When result is undefined, we don't want any `done` callbacks of
                // the deferred being called. So reject instead of resolve.
                deferredModalResult.rejectWith(context, []);
            }
        }
    };
};

var showTwitterBootstrapModal = function ($ui) {
    // Display the modal UI using Twitter Bootstrap's modal plug-in.
    $ui.modal({
        // Clicking the backdrop, or pressing Escape, shouldn't automatically close the modal by default.
        // The view model should remain in control of when to close.
        backdrop: "static",
        keyboard: false
    });
};

var whenModalResultCompleteThenHideUI = function (deferredModalResult, $ui) {
    // When modal is closed (with or without a result)
    // Then always hide the UI.
    deferredModalResult.always(function () {
        $ui.modal("hide");
    });
};

var whenUIHiddenThenRemoveUI = function ($ui) {
    // Hiding the modal can result in an animation.
    // The `hidden` event is raised after the animation finishes,
    // so this is the right time to remove the UI element.
    $ui.on("hidden", function () {
        // Call ko.cleanNode before removal to prevent memory leaks.
        $ui.each(function (index, element) { ko.cleanNode(element); });
        $ui.remove();
    });
};