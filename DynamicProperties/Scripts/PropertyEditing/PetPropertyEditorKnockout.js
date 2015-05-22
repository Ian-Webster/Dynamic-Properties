//Model for an individual property
function PetPropertyModel(data) {

    var self = this;

    self.PropertyID = data.Property.PropertyID;
    self.PropertyLabel = data.Property.PropertyName;
    self.PropertyValue = data.Value;
    self.PropertyValue_Bool = (data.Value == "True") ? true : false;
    self.PropertyName = 'text_' + data.Property.PropertyID; //id is text_ + the id of the property
    self.PropertyType = data.Property.PropertyType;
    
}

//Model for a pet
function PetModel() {
    var self = this;
    self.PetID = ko.observable();
    self.PetName = ko.observable();
    self.PetTypeID = ko.observable();
    self.Properties = ko.observableArray();
}

//Model for errors
function ErrorModel(data) {
    var self = this;
    self.ErrorMessage = data;
}

//View model - had to separate pet into a separate class because the safe function 
//expects self.<something> rather than just self
function PetViewModel() {

    var self = this;
    self.Pet = ko.observable(new PetModel());
    self.Errors = ko.observableArray();

    //note this is the correct way to set properties using <someobject>(<some value>) see
    //http://ryanrahlf.com/getting-started-with-knockout-js-3-things-to-know-on-day-one/
    //also note that if you want to refer to a child object you need to use <someobject>()
    self.Pet().PetID($('#PetID').val());
    self.Pet().PetTypeID($('#PetTypeID').val());
    self.Pet().PetName($('#Name').val());

    //this function get's the properties for the selected pet type
    self.PetTypeChanged = function () {
        //reset pet type to the selected value
        self.Pet().PetTypeID($('#PetTypeID').val());
        //perform JSON call too Home controller "ListPropertiesForPet" method
        //added "unique" parameter to the end of the string to prevent browsers from caching the Json result
        var d = new Date();
        var url = $('#RootUrl').val() + "Home/ListPropertiesForPet?petId=" + self.Pet().PetID() + "&petType=" + self.Pet().PetTypeID() + "&unique=" + d.getMilliseconds();
        $.getJSON(url, function (data) {
            //get the properties from the JSON result
            var mappedProperties = $.map(data, function (item) { return new PetPropertyModel(item) });
            //reset the properties for the pet view model
            self.Pet().Properties(mappedProperties);
            //reset the errors
            self.Errors([]);
        }).fail(function (jqxhr, textStatus, error) {
            //handler in the event of an error
            var err = textStatus + ', ' + error;
            alert("Request Failed: " + err);
        });

    };

    //saves changes made to a pet
    self.Save = function () {       
        //send ajax request to the controller
        $.ajax(location.href, {
            data: ko.toJSON({ Pet: self.Pet }),
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

    };

    //set the initial pet properties
    self.PetTypeChanged();

}

//apply knockout binding of the pet view model
ko.applyBindings(new PetViewModel());