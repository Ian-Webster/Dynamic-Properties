using DynamicProperties.Models;
using DynamicProperties.Models.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DynamicProperties.Controllers
{
    public class HomeController : Controller
    {

        private List<PetVM> Pets
        {
            get
            {
                return Session["Pets"] as List<PetVM>;
            }
            set
            {
                Session["Pets"] = value;
            }
        }

        private List<PetTypeVM> PetTypes
        {
            get
            {
                return Session["PetTypes"] as List<PetTypeVM>;
            }
            set
            {
                Session["PetTypes"] = value;
            }
        }

        #region pet types

        public ActionResult AddPetType()
        {
            var petType = new PetTypeVM()
            {
                PetTypeID = Guid.Empty,
                Properties = new List<PropertyVM>()
            };
            return View(petType);
        }
        [HttpPost]
        public JsonResult AddPetType(PetTypeVM petType)
        {
            List<string> errorList = new List<string>();
            if (SavePetType(petType))
            {
                return Json(new { result = true, errors = errorList }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                errorList = ModelState.Values.SelectMany(s => s.Errors).Select(e => e.ErrorMessage).ToList();
                return Json(new { result = false, errors = errorList }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult EditPetType(Guid petTypeId)
        {
            return View(PetTypes.Where(pt => pt.PetTypeID == petTypeId).First());
        }
        [HttpPost]
        public JsonResult EditPetType(PetTypeVM petType)
        {
            List<string> errorList = new List<string>();
            if (SavePetType(petType))
            {
                return Json(new { result = true, errors = errorList }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                errorList = ModelState.Values.SelectMany(s => s.Errors).Select(e => e.ErrorMessage).ToList();
                return Json(new { result = false, errors = errorList }, JsonRequestBehavior.AllowGet);
            }
        }

        private bool SavePetType(PetTypeVM petType)
        {
            bool result = true;
            if (petType.Name == string.Empty)
            {
                result = false;
                ModelState.AddModelError(string.Empty, "You must enter the pet type name");
            }
            if (petType.Properties == null || petType.Properties.Count == 0)
            {
                result = false;
                ModelState.AddModelError(string.Empty, "You must enter some properties");
            }
            if (petType.PetTypeID == Guid.Empty)
            {
                petType.PetTypeID = Guid.NewGuid();
                PetTypes.Add(petType);
            }
            else
            {
                PetTypeVM vm = PetTypes.Where(pt => pt.PetTypeID == petType.PetTypeID).First();
                PetTypes.Remove(vm);
                PetTypes.Add(petType);
            }
            return result;
        }

        #endregion

        #region pets

        public ActionResult Index()
        {
            if (Pets == null)
            {
                Pets = new List<PetVM>();
            }
            if (PetTypes == null)
            {
                PetTypes = new List<PetTypeVM>();
                var petType = new PetTypeVM()
                {
                    Name = "Dog",
                    PetTypeID = Guid.NewGuid(),
                    Properties = new List<PropertyVM>()
                };
                petType.Properties.Add(new PropertyVM() { PropertyID = 1, PropertyName = "How often do you walk the dog a day", PropertyType = PropertyType.Numeric, Required = true });
                PetTypes.Add(petType);
                petType = new PetTypeVM()
                {
                    Name = "Cat",
                    PetTypeID = Guid.NewGuid(),
                    Properties = new List<PropertyVM>()
                };
                petType.Properties.Add(new PropertyVM() { PropertyID = 1, PropertyName = "Do you let your cat out", PropertyType = PropertyType.Boolean, Required = false });
                PetTypes.Add(petType);
            }
            return View(new HomeVM() { Pets = Pets, PetTypes = PetTypes });
        }

        public ActionResult AddPet()
        {
            if (PetTypes == null || PetTypes.Count == 0)
            {
                return RedirectToAction("AddPetType");
            }
            else
            {
                SetPetTypes();
                var pet = new PetVM()
                {
                    PetTypeID = PetTypes[0].PetTypeID,
                    Properties = ListPropertiesForNewPet(PetTypes[0].PetTypeID)
                };
                return View(pet);
            }

        }
        [HttpPost]
        public JsonResult AddPet(PetViewModel pet)
        {
            List<string> errorList = new List<string>();
            if (pet != null)
            {
                if (SavePet(pet))
                {
                    return Json(new { result = true, errors = errorList }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    SetPetTypes();
                    errorList = ModelState.Values.SelectMany(s => s.Errors).Select(e => e.ErrorMessage).ToList();
                    return Json(new { result = false, errors = errorList }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                throw new Exception("pet was null");
            }
        }

        public ActionResult EditPet(Guid PetID)
        {
            SetPetTypes();
            return View(Pets.Where(p => p.PetID == PetID).First());
        }
        [HttpPost]
        public JsonResult EditPet(PetViewModel pet)
        {
            List<string> errorList = new List<string>();
            if (pet != null)
            {
                if (SavePet(pet))
                {
                    return Json(new { result = true, errors = errorList }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    SetPetTypes();
                    errorList = ModelState.Values.SelectMany(s => s.Errors).Select(e => e.ErrorMessage).ToList();
                    return Json(new { result = false, errors = errorList }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                throw new Exception("pet was null");
            }
        }

        public JsonResult ListPropertiesForPet(Guid petId, Guid petType)
        {
            if (petId == Guid.Empty)
            {
                return Json(ListPropertiesForNewPet(petType), JsonRequestBehavior.AllowGet);
            }
            else
            {
                var pet = Pets.Where(p => p.PetID == petId).First();
                if (pet.PetTypeID != petType)
                {
                    //pet type changed
                    return Json(ListPropertiesForNewPet(petType), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(Pets.Where(p => p.PetID == petId).First().Properties, JsonRequestBehavior.AllowGet);
                }
                
            }
        }

        private List<PetPropertyVM> ListPropertiesForNewPet(Guid petType)
        {
            var result = new List<PetPropertyVM>();
            var properties = PetTypes.Where(pt => pt.PetTypeID == petType).First().Properties;
            var petProperty = new PetPropertyVM();

            foreach (var prop in properties)
            {
                result.Add(new PetPropertyVM()
                {
                    ID = Guid.NewGuid(),
                    Property = prop,
                    Value = string.Empty
                });
            }

            return result;
        }        

        private bool SavePet(PetViewModel petToSave)
        {
            bool result = true;
            PetVM pet = null;

            if (String.IsNullOrWhiteSpace(petToSave.PetName))
            {
                result = false;
                ModelState.AddModelError(string.Empty, "You must enter the pet name");
            }

            if (petToSave.PetID == Guid.Empty)
            {
                //new pet
                pet = new PetVM()
                {
                    Name = petToSave.PetName,
                    PetTypeID = petToSave.PetTypeID,
                    Properties = ListPropertiesForNewPet(petToSave.PetTypeID),
                    PetTypeName = PetTypes.Where(pt => pt.PetTypeID == petToSave.PetTypeID).First().Name
                };
            }
            else
            {
                //existing pet
                pet = Pets.Where(p => p.PetID == petToSave.PetID).First();
                pet.Name = petToSave.PetName;
                pet.PetTypeID = petToSave.PetTypeID;
                pet.Properties = ListPropertiesForNewPet(pet.PetTypeID);
            }

            foreach(var property in pet.Properties)
            {
                if (petToSave.Properties.Any(p => p.PropertyID == property.Property.PropertyID && (!String.IsNullOrWhiteSpace(p.PropertyValue)) || property.Property.PropertyType == PropertyType.Boolean))
                {
                    switch (property.Property.PropertyType)
                    {
                        case PropertyType.Boolean:
                            property.Value = petToSave.Properties.Where(p => p.PropertyID == property.Property.PropertyID).First().PropertyValue_Bool.ToString();
                            break;
                        case PropertyType.Numeric:
                            string value = petToSave.Properties.Where(p => p.PropertyID == property.Property.PropertyID).First().PropertyValue;
                            int test;
                            if (int.TryParse(value, out test))
                            {
                                property.Value = value;
                            }
                            else
                            {
                                result = false;
                                ViewData.ModelState.AddModelError(string.Empty, "The value for " + property.Property.PropertyName + " must be numeric");
                            }
                            break;
                        case PropertyType.Textbox:
                            property.Value = petToSave.Properties.Where(p => p.PropertyID == property.Property.PropertyID).First().PropertyValue;
                            break;
                    }                    
                }
                else if (property.Property.Required)
                {
                    result = false;
                    ViewData.ModelState.AddModelError(string.Empty, "You must provide a value for " + property.Property.PropertyName);
                }
            }

            if (result && pet.PetID == Guid.Empty)
            {
                pet.PetID = Guid.NewGuid();
                Pets.Add(pet);
            }

            return result;

        }

        private void SetPetTypes()
        {
            //PetTypes
            var items = new List<SelectListItem>();
            PetTypes.ForEach(pt =>
                {
                    items.Add(new SelectListItem() { Text = pt.Name, Value = pt.PetTypeID.ToString() });
                }
            );
            ViewBag.PetTypes = items;
        }

        #endregion

    }
}