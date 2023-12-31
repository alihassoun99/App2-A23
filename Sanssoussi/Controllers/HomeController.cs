﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using System.Data.SqlClient;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using Sanssoussi.Areas.Identity.Data;
using Sanssoussi.Models;

using System.Xml.Linq;
using Microsoft.AspNetCore.Authorization;
using Sanssoussi.Areas.Identity.Data.Config.Const;

namespace Sanssoussi.Controllers
{
    public class HomeController : Controller
    {
        private readonly SqliteConnection _dbConnection;

        private readonly ILogger<HomeController> _logger;

        private readonly UserManager<SanssoussiUser> _userManager;

        public HomeController(ILogger<HomeController> logger, UserManager<SanssoussiUser> userManager, IConfiguration configuration)
        {
            this._logger = logger;
            this._userManager = userManager;
            this._dbConnection = new SqliteConnection(configuration.GetConnectionString("SanssoussiContextConnection"));
        }



        [AllowAnonymous]
        public IActionResult Index()
        {
            this.ViewData["Message"] = "Parce que marcher devrait se faire SansSoussi";
            return this.View();
        }

        [HttpGet]
        [Authorize(Roles = Rules.Client + "," + Rules.Admin)]
        public async Task<IActionResult> Comments()
        {
            var comments = new List<string>();

            var user = await this._userManager.GetUserAsync(this.User);
            if (user == null)
            {
                return this.View(comments);
            }


            ///////////////////////////////////////////////////////////
            // A03:2021-Injection
            var idUser = user.Id;
            var cmd = new SqliteCommand(
                            $"Select " +
                            $"Comment " +
                            $"from " +
                            $"Comments " +
                            $"where " +
                            $"UserId =@idUser"
                            , this._dbConnection);

            // passe le parametre dans la commande
            // parameterized queries empeche les injeciton SQL

            cmd.Parameters.AddWithValue("idUser", idUser);

            this._dbConnection.Open();
            var rd = await cmd.ExecuteReaderAsync();

            while (rd.Read())
            {
                comments.Add(rd.GetString(0));
            }

            rd.Close();
            this._dbConnection.Close();

            return this.View(comments);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Rules.Client + "," + Rules.Admin)]
        public async Task<IActionResult> Comments(string comment)
        {

            // test : R-0
            Console.WriteLine("R-0");

            // A03:2021 Injection : data validation Function

            if (string.IsNullOrEmpty(comment))
            {
                var user = await this._userManager.GetUserAsync(this.User);
                // journalisation de tout les commentaire
                // A09:2021
                _logger.LogWarning("Commentaire Malveillant ajoute par UserId : {user.Id} !!!!!", user.Id);
                _logger.LogWarning("Time : {DateTime.Now}", DateTime.Now);
                return null;
            }
            else
            {
                // pour depaser le risk d'executer des commandes SQL tell que : ( ' UNION , etc)

                comment = comment.Replace("'", "''");

                // validation des donnees
                var user = await this._userManager.GetUserAsync(this.User);
                if (user == null)
                {
                    throw new InvalidOperationException("Vous devez vous connecter");
                }

                // entree des variable et la commande SQL
                // parameterized queries empeche les injeciton SQL

                var guidNewGuide = Guid.NewGuid();
                var IdUser = user.Id;
                var cmd = new SqliteCommand( $"insert into Comments (UserId, CommentId, Comment) Values (@userId, @guidNewGuid, @comment)",this._dbConnection);

                // ajout des parametres a la commande SQL
                cmd.Parameters.AddWithValue("userId", IdUser);
                cmd.Parameters.AddWithValue("guidNewGuid", guidNewGuide);
                cmd.Parameters.AddWithValue("comment", comment);

                this._dbConnection.Open();
                await cmd.ExecuteNonQueryAsync();

                // journalisation de tout les commentaire
                // A09:2021 : Journalisation
                _logger.LogWarning("Commentaire ajoute par UserId : {user.Id} ", IdUser);
                _logger.LogWarning("Time : {DateTime.Now}", DateTime.Now );

                return this.Ok("Commentaire ajouté");
            }

        }

        [Authorize(Roles = Rules.Client + "," + Rules.Admin)]
        public async Task<IActionResult> Search(string searchData)
        {
            var searchResults = new List<string>();

            var user = await this._userManager.GetUserAsync(this.User);
            if (user == null || string.IsNullOrEmpty(searchData))
            {
                // journalisation de tout les recherche
                // A09:2021
                _logger.LogWarning("recherche Malveillant possible par UserId : {user.Id} !!!!!", user.Id);
                _logger.LogWarning("Time : {DateTime.Now}", DateTime.Now);
                return this.View(searchResults);
            }

            // A03:2021 : Injection SQL
            // parameterized queries empeche les injeciton SQL
            var idUser = user.Id;
            var cmd = new SqliteCommand($"Select Comment from Comments where UserId = @userId and Comment like %@searchData%", this._dbConnection);
            cmd.Parameters.AddWithValue("userId", idUser);
            cmd.Parameters.AddWithValue("searchData", searchData);
            this._dbConnection.Open();
            var rd = await cmd.ExecuteReaderAsync();
            while (rd.Read())
            {

                searchResults.Add(rd.GetString(0));
            }

            // journalisation de tout les recherche
            // A09:2021 : Journalisation
            _logger.LogWarning("Commentaire ajoute par UserId : {idUser} ", idUser);
            _logger.LogWarning("Time : {DateTime.Now}", DateTime.Now);

            rd.Close();
            this._dbConnection.Close();

            return this.View(searchResults);
        }

        [AllowAnonymous]
        public IActionResult About()
        {
            return this.View();
        }

        [AllowAnonymous]
        public IActionResult Privacy()
        {
            return this.View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return this.View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier });
        }

        [HttpGet]
        [Authorize(Roles = Rules.Admin)]
        public IActionResult Emails()
        {
            return this.View();
        }

        [HttpPost]
        [Authorize(Roles = Rules.Admin)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Emails(object form)
        {
            var searchResults = new List<string>();

            var user = await this._userManager.GetUserAsync(this.User);
            var roles = await this._userManager.GetRolesAsync(user);
            if (roles.Contains("admin"))
            {
                var cmd = new SqliteCommand("select Email from AspNetUsers", this._dbConnection);
                this._dbConnection.Open();
                var rd = await cmd.ExecuteReaderAsync();
                while (rd.Read())
                {
                    searchResults.Add(rd.GetString(0));
                }

                rd.Close();

                this._dbConnection.Close();
            }

            return this.Json(searchResults);
        }
    }
}