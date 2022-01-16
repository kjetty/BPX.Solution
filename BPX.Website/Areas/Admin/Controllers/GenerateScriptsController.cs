using BPX.Service;
using BPX.Utils;
using BPX.Website.Controllers;
using BPX.Website.CustomCode.Authorize;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BPX.Website.Areas.Admin.Controllers
{
	[Area("Admin")]
    public class GenerateScriptsController : BaseController<GenerateScriptsController>
    {
        private readonly IPermitService permitService;
        private readonly IRoleService roleService;

        public GenerateScriptsController(IPermitService permitService, IRoleService roleService)
        {
            this.permitService = permitService;
            this.roleService = roleService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Permit(Permits.Admin.GenerateScripts.PermitConstants)]
        public IActionResult PermitConstants()
        {
            return View();
        }

        [HttpPost]
        [Permit(Permits.Admin.GenerateScripts.PermitConstants)]
        public IActionResult PermitConstants(int id)
        {
            Generate_PermitConstants();

            // set alert
            ShowAlert(AlertType.Success, "PermitConstans.cs  is successfully generated.");

            return View();
        }

        public void Generate_PermitConstants()
        {
            string path = "C:/temp";
            string fileName = "PermitConstants.cs";
            string tempString = string.Empty;

            var permitAreaList = permitService.GetRecordsByFilter(c => c.StatusFlag.Equals(RecordStatus.Active)).OrderBy(c => c.PermitArea).Select(c => c.PermitArea).Distinct().ToList();

            tempString += "namespace BPX.Website.CustomCode.Authorize" + Environment.NewLine;
            tempString += "{ " + Environment.NewLine;
            tempString += "\t" + "public static class Permits" + Environment.NewLine;
            tempString += "\t" + "{ " + Environment.NewLine;

            foreach (string itemPermitArea in permitAreaList)
            {
                tempString += "\t" + "\t" + "public static class " + itemPermitArea + Environment.NewLine;
                tempString += "\t" + "\t" + "{ " + Environment.NewLine;

                // db call
                var permitControllerList = permitService.GetRecordsByFilter(c => c.StatusFlag.Equals(RecordStatus.Active) && c.PermitArea.Equals(itemPermitArea)).OrderBy(c => c.PermitController).Select(c => c.PermitController).Distinct().ToList();

                foreach (string itemControllerArea in permitControllerList)
                {
                    tempString += "\t" + "\t" + "\t" + "public static class " + itemControllerArea + Environment.NewLine;
                    tempString += "\t" + "\t" + "\t" + "{ " + Environment.NewLine;

                    // db call
                    var permitObjList = permitService.GetRecordsByFilter(c => c.StatusFlag.Equals(RecordStatus.Active) && c.PermitArea.Equals(itemPermitArea) && c.PermitController.Equals(itemControllerArea)).OrderBy(c => c.PermitName).Select(c => new { c.PermitID, c.PermitName, c.PermitEnum }).OrderBy(c => c.PermitName).ToList();

                    foreach (var itemPermitObj in permitObjList)
                    {
                        tempString += "\t" + "\t" + "\t" + "\t" + "public const int " + itemPermitObj.PermitName + " = " + itemPermitObj.PermitID + "; \t\t\t //" + itemPermitObj.PermitEnum + Environment.NewLine;
                    }

                    tempString += "\t" + "\t" + "\t" + "} " + Environment.NewLine + Environment.NewLine;
                }

                tempString += "\t" + "\t" + "} " + Environment.NewLine + Environment.NewLine;
            }

            tempString += "\t" + "} " + Environment.NewLine;
            tempString += "} " + Environment.NewLine;

            WriteToFile(path, fileName, tempString);
        }

        private static void WriteToFile(string path, string fileName, string fileData)
        {
            System.IO.File.WriteAllText(path + "/" + fileName, String.Empty);

            //if (fileName == "PermitConstants.cs")
                System.IO.File.AppendAllText(path + "/" + fileName, "//This file is auto generated on " + DateTime.Now.ToString() + Environment.NewLine);
            //else
            //    System.IO.File.AppendAllText(path + "/" + fileName, $"--This file is auto generated on {DateTime.Now}{Environment.NewLine}");

            System.IO.File.AppendAllText(path + "/" + fileName, fileData);
        }
    }
}