﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LaboGSB.Models.DAO.DAOCompteRendu;
using LaboGSB.Models.DAO.DAOGestionFrais;
using LaboGSB.Models.BO.Personne;
using LaboGSB.Models.BO.BOCompteRendu;
using LaboGSB.Models.BO.BOGestionFrais;


namespace GSB.Controllers
{
    public class HomeController : Controller
    {

        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        public ActionResult Accueil(string id)
        {
            ActionResult retour = View();

            CompteRenduDAO crDao = new CompteRenduDAO();
            EtablissementDAO etabDao = new EtablissementDAO();
            ContactDAO contactDao = new ContactDAO();
            ProduitDAO produitDao = new ProduitDAO();
            List<CompteRendu> listeCompteRendus = crDao.RetournerTousLesCompteRendus();
            List<Etablissement> listeEtablissements = etabDao.RetournerTousLesEtablissements();
            List<Contact> listeContacts = contactDao.RetournerTousLesContacts();
            List<Produit> listeProduits = produitDao.RetournerTousLesProduits();
            ViewBag.listeCompteRendus = listeCompteRendus;
            ViewBag.listeEtablissements = listeEtablissements;
            ViewBag.listeContacts = listeContacts;
            ViewBag.listeProduits = listeProduits;
            ViewBag.afficherListeCompteRendus = false;
            ViewBag.afficherListeEtab = false;
            ViewBag.afficherListeContacts = false;
            ViewBag.afficherListeProduits = false;


            if (!String.IsNullOrWhiteSpace(id))
            {
                if (id == "Listedesetablissements")
                {
                    ViewBag.afficherListeEtab = true;
                }
                else if(id == "Listedescontacts")
                {
                    ViewBag.afficherListeContacts = true;
                }
                else if (id == "Listedesproduits")
                {
                    ViewBag.afficherListeProduits = true;
                }
                else
                {
                    ViewBag.afficherListeCompteRendus = true;
                }
            }

                return retour;
        }

        public ActionResult FormCompteRendu(String id)
        {
            CompteRenduDAO crDao = new CompteRenduDAO();
            VisiteurMedicalDAO visiteurMedicalDao = new VisiteurMedicalDAO();
            ContactDAO contactDao = new ContactDAO();
            EtablissementDAO etabDao = new EtablissementDAO();
            ProduitDAO produitDao = new ProduitDAO();

            CompteRendu cr = new CompteRendu();
            ViewBag.cr = cr;

            List<VisiteurMedical> listeVisiteursMedicaux = visiteurMedicalDao.RetournerTousLesVisiteursMedicaux();
            ViewBag.listeVisiteursMedicaux = listeVisiteursMedicaux;

            List<Contact> listeContacts = contactDao.RetournerTousLesContacts();
            ViewBag.listeContacts = listeContacts;

            List<Etablissement> listeEtablissements = etabDao.RetournerTousLesEtablissements();
            ViewBag.listeEtablissements = listeEtablissements;

            List<Produit> listeProduits = produitDao.RetournerTousLesProduits();
            ViewBag.listeProduits = listeProduits;

            ActionResult retour = View();

            if (Request.HttpMethod == "POST")
            {
                int idCompteRendu = Int32.Parse(Request.Form["idCompteRendu"]);
                VisiteurMedical visiteurMedical = listeVisiteursMedicaux.Find(vm => vm.Id == Int32.Parse(Request.Form["visiteurMedical"]));
                Contact contact = listeContacts.Find(cont => cont.Id == Int32.Parse(Request.Form["contact"]));
                Etablissement etablissement = listeEtablissements.Find(etab => etab.Id == Int32.Parse(Request.Form["etablissement"]));
                string titre = Request.Form["titre"];
                string contenu = Request.Form["contenu"];
                DateTime date = Convert.ToDateTime(Request.Form["date"]);
                List<Echantillon> listeEchantillon = new List<Echantillon>();


                cr = new CompteRendu(idCompteRendu, visiteurMedical, contact, etablissement, titre, contenu, date, listeEchantillon);

                if (idCompteRendu == 0)
                {
                    crDao.Create(cr);
                }
                else
                {
                    crDao.Update(cr);
                }
                ViewBag.cr = cr;

                retour = View("FicheCompteRendu");

            }
            else
            {
                if (!String.IsNullOrWhiteSpace(id))
                {
                    if (Int32.TryParse(id, out int idCompteRendu))
                    {
                        if (etabDao.Read(idCompteRendu) != null)
                        {
                            cr = crDao.Read(idCompteRendu);
                            if ((Request.HttpMethod == "GET") && (Request.Params["action"] != null))
                            {
                                if (Request.Params["action"] == "del")
                                {
                                    crDao.Delete(cr);
                                    retour = RedirectToAction("FicheCompteRendu");
                                }
                            }
                            else
                            {
                                ViewBag.cr = cr;
                            }
                        }
                    }
                }
            }

            return retour;
        }

        public ActionResult FicheCompteRendu(String id)
        {
            CompteRenduDAO crDao = new CompteRenduDAO();
            ActionResult retour = RedirectToAction("Accueil", new { id = "Listedescompterendus" });

            if (!String.IsNullOrWhiteSpace(id))
            {
                if (Int32.TryParse(id, out int idCompteRendu))
                {
                    if (crDao.Read(idCompteRendu) != null)
                    {
                        CompteRendu cr = crDao.Read(idCompteRendu);
                        ViewBag.cr = cr;
                        retour = View();
                    }
                }
            }

            return retour;
        }

        public ActionResult FormEtablissement(string id)
        {
            EtablissementDAO etabDao = new EtablissementDAO();
            TypeEtablissementDAO typesEtabDao = new TypeEtablissementDAO();
            Etablissement etab = new Etablissement();
            ViewBag.etablissement = etab;
            List<TypeEtablissement> listeTypesEtablissement = typesEtabDao.RetournerTousLesTypesEtablissement();
            ViewBag.listeTypesEtablissement = listeTypesEtablissement;
            ActionResult retour = View();

            if (Request.HttpMethod == "POST")
            {
                int idEtab = Int32.Parse(Request.Form["idEtab"]);
                string nom = Request.Form["nom"];
                string adresse = Request.Form["adresse"];
                string mel = Request.Form["adresseMel"];
                string numeroTelephone = Request.Form["numeroTelephone"];
                int idType = Int32.Parse(Request.Form["idType"]);
                TypeEtablissement typeEtab = listeTypesEtablissement.Find(tp => tp.Id == idType);


                etab = new Etablissement(idEtab, nom, adresse, numeroTelephone, mel, typeEtab);

                if (idEtab == 0)
                {
                    etabDao.Create(etab);
                }
                else
                {
                    etabDao.Update(etab);
                }
                ViewBag.etablissement = etab;

                retour = View("FicheEtablissement");

            }
            else
            {
                if (!String.IsNullOrWhiteSpace(id))
                {
                    if (Int32.TryParse(id, out int idEtab))
                    {
                        if (etabDao.Read(idEtab) != null)
                        {
                            etab = etabDao.Read(idEtab);
                            if ((Request.HttpMethod == "GET") && (Request.Params["action"] != null))
                            {
                                if (Request.Params["action"] == "del")
                                {
                                    etabDao.Delete(etab);
                                    retour = RedirectToAction("FicheEtablissement");
                                }
                            }
                            else
                            {
                                ViewBag.etablissement = etab;
                            }
                        }
                    }
                }
            }

            return retour;
        }

        public ActionResult FicheEtablissement(String id)
        {
            EtablissementDAO etabDao = new EtablissementDAO();
            ActionResult retour = RedirectToAction("Accueil", new { id = "Listedesetablissements" });

            if (!String.IsNullOrWhiteSpace(id))
            {
                if (Int32.TryParse(id, out int idEtab))
                {
                    if (etabDao.Read(idEtab) != null)
                    {
                        Etablissement etab = etabDao.Read(idEtab);
                        ViewBag.etablissement = etab;
                        retour = View();
                    }
                }
            }

            return retour;
        }

        public ActionResult FormContact(string id)
        {
            ContactDAO contactDao = new ContactDAO();
            Contact contact = new Contact();
            ViewBag.contact = contact;
            ActionResult retour = View();

            if (Request.HttpMethod == "POST")
            {
                int idContact = Int32.Parse(Request.Form["idContact"]);
                string nom = Request.Form["nom"];
                string prenom = Request.Form["prenom"];
                string mel = Request.Form["adresseMel"];
                string numeroTelephone = Request.Form["numeroTelephone"];
                string poste = Request.Form["poste"];
                string commentaire = Request.Form["commentaire"];
                List<Etablissement> listeEmployeur = new List<Etablissement>();
                contact = new Contact(poste, commentaire, listeEmployeur, idContact, nom, prenom, mel, numeroTelephone);

                if (idContact == 0)
                {
                    contactDao.Create(contact);
                }
                else
                {
                    contactDao.Update(contact);
                }
                ViewBag.contact = contact;

                retour = View("FicheContact");

            }
            else
            {
                if (!String.IsNullOrWhiteSpace(id))
                {
                    if (Int32.TryParse(id, out int idContact))
                    {
                        if (contactDao.Read(idContact) != null)
                        {
                            contact = contactDao.Read(idContact);
                            if ((Request.HttpMethod == "GET") && (Request.Params["action"] != null))
                            {
                                if (Request.Params["action"] == "del")
                                {
                                    contactDao.Delete(contact);
                                    retour = RedirectToAction("FicheEtablissement");
                                }
                            }
                            else
                            {
                                ViewBag.contact = contact;
                            }
                        }
                    }
                }
            }

            return retour;
        }

        public ActionResult FicheContact(String id)
        {
            ContactDAO contactDao = new ContactDAO();
            ActionResult retour = RedirectToAction("Accueil", new { id = "Listedescontacts" });

            if (!String.IsNullOrWhiteSpace(id))
            {
                if (Int32.TryParse(id, out int idContact))
                {
                    if (contactDao.Read(idContact) != null)
                    {
                        Contact contact = contactDao.Read(idContact);
                        ViewBag.contact = contact;
                        retour = View();
                    }
                }
            }

            return retour;
        }

        public ActionResult FormProduit(String id)
        {
            ProduitDAO produitDao = new ProduitDAO();
            Produit produit = new Produit();
            ViewBag.produit = produit;
            ActionResult retour = View();

            if (Request.HttpMethod == "POST")
            {
                int idProduit = Int32.Parse(Request.Form["idProduit"]);
                string designation = Request.Form["designation"];
                string description = Request.Form["description"];
                int quantite = Int32.Parse(Request.Form["quantite"]);
                double tarif = double.Parse(Request.Form["tarif"]);

                produit = new Produit(idProduit, designation, description, quantite, tarif);

                if (idProduit == 0)
                {
                    produitDao.Create(produit);
                }
                else
                {
                    produitDao.Update(produit);
                }
                ViewBag.produit = produit;

                retour = View("FicheProduit");

            }
            else
            {
                if (!String.IsNullOrWhiteSpace(id))
                {
                    if (Int32.TryParse(id, out int idProduit))
                    {
                        if (produitDao.Read(idProduit) != null)
                        {
                            produit = produitDao.Read(idProduit);
                            if ((Request.HttpMethod == "GET") && (Request.Params["action"] != null))
                            {
                                if (Request.Params["action"] == "del")
                                {
                                    produitDao.Delete(produit);
                                    retour = RedirectToAction("FicheProduit");
                                }
                            }
                            else
                            {
                                ViewBag.produit = produit;
                            }
                        }
                    }
                }
            }

            return retour;
        }

        public ActionResult FicheProduit(String id)
        {
            ProduitDAO produitDao = new ProduitDAO();
            ActionResult retour = RedirectToAction("Accueil", new { id = "Listedesproduits" });

            if (!String.IsNullOrWhiteSpace(id))
            {
                if (Int32.TryParse(id, out int idProduit))
                {
                    if (produitDao.Read(idProduit) != null)
                    {
                        Produit produit = produitDao.Read(idProduit);
                        ViewBag.produit = produit;
                        retour = View();
                    }
                }
            }

            return retour;
        }

    }
}