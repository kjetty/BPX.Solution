using BPX.Domain.DbModels;
using BPX.Service;
using BPX.Utils;
using BPX.Website.Controllers;
using BPX.Website.CustomCode.Authorize;
using Microsoft.AspNetCore.Hosting;
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
        private string pathPermitConstants;

        public PermitsGeneratorController(ILogger<PermitsGeneratorController> logger, ICoreService coreService, IPermitService permitService, IRoleService roleService) : base(logger, coreService)
		{
            this.permitService = permitService;
            this.roleService = roleService;
            this.pathPermitConstants = coreService.GetConfiguration().GetSection("AppSettings").GetSection("PathPermitConstants").Value;
        }

        [HttpGet]
        [Permit(Permits.Developer.PermitsGenerator.Index)]
        public IActionResult Index()
        {
            ViewBag.pathPermitConstants = pathPermitConstants;
            
            return View();
        }

        [HttpPost]
        [Permit(Permits.Developer.PermitsGenerator.Index)]
        public IActionResult Index(int id)
        {
			GeneratePermitConstants();
           
            // set alert
            ShowAlertBox(AlertType.Success, "PermitConstans.cs  is successfully generated at " + pathPermitConstants);

			return View();
        }

        private void GeneratePermitConstants()
        {
            string path = pathPermitConstants;
            string fileName = "PermitConstants.cs";
            string tempString = string.Empty;

            List<string> listPermitArea = permitService.GetRecordsByFilter(c => c.StatusFlag.ToUpper().Equals(RecordStatus.Active.ToUpper())).OrderBy(c => c.PermitArea).Select(c => c.PermitArea).Distinct().ToList();

            tempString += "namespace BPX.Website.CustomCode.Authorize" + Environment.NewLine;
            tempString += "{ " + Environment.NewLine;
            tempString += "\t" + "public static class Permits" + Environment.NewLine;
            tempString += "\t" + "{ " + Environment.NewLine;

            foreach (string itemPermitArea in listPermitArea)
            {
                tempString += "\t" + "\t" + "public static class " + itemPermitArea + Environment.NewLine;
                tempString += "\t" + "\t" + "{ " + Environment.NewLine;

                // db call
                List<string> listPermitController = permitService.GetRecordsByFilter(c => c.StatusFlag.ToUpper().Equals(RecordStatus.Active.ToUpper()) && c.PermitArea.ToUpper().Equals(itemPermitArea.ToUpper())).OrderBy(c => c.PermitController).Select(c => c.PermitController).Distinct().ToList();

                foreach (string itemPermitController in listPermitController)
                {
                    tempString += "\t" + "\t" + "\t" + "public static class " + itemPermitController + Environment.NewLine;
                    tempString += "\t" + "\t" + "\t" + "{ " + Environment.NewLine;

                    // db call
                    List<Permit> listPermit = permitService.GetRecordsByFilter(c => c.StatusFlag.ToUpper().Equals(RecordStatus.Active.ToUpper()) && c.PermitArea.ToUpper().Equals(itemPermitArea.ToUpper()) && c.PermitController.ToUpper().Equals(itemPermitController.ToUpper())).OrderBy(c => c.PermitName).OrderBy(c => c.PermitName).ToList();

                    foreach (Permit itemPermit in listPermit)
                    {
                        tempString += "\t" + "\t" + "\t" + "\t" + "public const int " + itemPermit.PermitName + " = " + itemPermit.PermitId + "; \t\t\t //" + itemPermit.PermitEnum + Environment.NewLine;
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
            System.IO.File.WriteAllText(path + "/" + fileName, string.Empty);
            System.IO.File.AppendAllText(path + "/" + fileName, "//This file is auto generated on " + DateTime.Now.ToString() + Environment.NewLine);
            System.IO.File.AppendAllText(path + "/" + fileName, fileData);
        }
    }
}
