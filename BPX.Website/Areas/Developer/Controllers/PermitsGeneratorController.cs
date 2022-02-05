﻿using BPX.Service;
using BPX.Utils;
using BPX.Website.Controllers;
using BPX.Website.CustomCode.Authorize;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BPX.Website.Areas.Developer.Controllers
{
    [Area("Developer")]
    public class PermitsGeneratorController : BaseController<PermitsGeneratorController>
	{
        private readonly IPermitService permitService;
        private readonly IRoleService roleService;

        public PermitsGeneratorController(ILogger<PermitsGeneratorController> logger, ICoreService coreService, IPermitService permitService, IRoleService roleService) : base(logger, coreService)
		{
            this.permitService = permitService;
            this.roleService = roleService;
        }

        [HttpGet]
        [Permit(Permits.Developer.PermitsGenerator.Index)]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Permit(Permits.Developer.PermitsGenerator.Index)]
        public IActionResult Index(int id)
        {
			GeneratePermitConstants();

			// set alert
			ShowAlertBox(AlertType.Success, "PermitConstans.cs  is successfully generated at c:\\temp");

			return View();
        }

        private void GeneratePermitConstants()
        {
            string path = "c:\\temp";
            string fileName = "PermitConstants.cs";
            string tempString = string.Empty;

            var permitAreaList = permitService.GetRecordsByFilter(c => c.StatusFlag.ToUpper().Equals(RecordStatus.Active.ToUpper())).OrderBy(c => c.PermitArea).Select(c => c.PermitArea).Distinct().ToList();

            tempString += "namespace BPX.Website.CustomCode.Authorize" + Environment.NewLine;
            tempString += "{ " + Environment.NewLine;
            tempString += "\t" + "public static class Permits" + Environment.NewLine;
            tempString += "\t" + "{ " + Environment.NewLine;

            foreach (string itemPermitArea in permitAreaList)
            {
                tempString += "\t" + "\t" + "public static class " + itemPermitArea + Environment.NewLine;
                tempString += "\t" + "\t" + "{ " + Environment.NewLine;

                // db call
                var permitControllerList = permitService.GetRecordsByFilter(c => c.StatusFlag.ToUpper().Equals(RecordStatus.Active.ToUpper()) && c.PermitArea.ToUpper().Equals(itemPermitArea.ToUpper())).OrderBy(c => c.PermitController).Select(c => c.PermitController).Distinct().ToList();

                foreach (string itemPermitController in permitControllerList)
                {
                    tempString += "\t" + "\t" + "\t" + "public static class " + itemPermitController + Environment.NewLine;
                    tempString += "\t" + "\t" + "\t" + "{ " + Environment.NewLine;

                    // db call
                    var permitObjList = permitService.GetRecordsByFilter(c => c.StatusFlag.ToUpper().Equals(RecordStatus.Active.ToUpper()) && c.PermitArea.ToUpper().Equals(itemPermitArea.ToUpper()) && c.PermitController.ToUpper().Equals(itemPermitController.ToUpper())).OrderBy(c => c.PermitName).Select(c => new { c.PermitId, c.PermitName, c.PermitEnum }).OrderBy(c => c.PermitName).ToList();

                    foreach (var itemPermitObj in permitObjList)
                    {
                        tempString += "\t" + "\t" + "\t" + "\t" + "public const int " + itemPermitObj.PermitName + " = " + itemPermitObj.PermitId + "; \t\t\t //" + itemPermitObj.PermitEnum + Environment.NewLine;
                    }

                    tempString += "\t" + "\t" + "\t" + "} " + Environment.NewLine + Environment.NewLine;
                }

                tempString += "\t" + "\t" + "} " + Environment.NewLine + Environment.NewLine;
            }

            tempString += "\t" + "} " + Environment.NewLine;
            tempString += "} " + Environment.NewLine;

            WriteToFile(path, fileName, tempString);

            //ShowAlertBox(AlertType.Success, "PermitConstans.cs  is successfully generated at C:\\temp");
        }

        private static void WriteToFile(string path, string fileName, string fileData)
        {
            System.IO.File.WriteAllText(path + "/" + fileName, String.Empty);
            System.IO.File.AppendAllText(path + "/" + fileName, "//This file is auto generated on " + DateTime.Now.ToString() + Environment.NewLine);
            System.IO.File.AppendAllText(path + "/" + fileName, fileData);
        }
    }
}