using System.Collections; using System.Collections.Generic; using UnityEngine; using UnityEngine.UI;
using UnityEngine.Networking; using SimpleJSON; using System.Linq;
using UnityEngine.EventSystems;

public class MainScript : MonoBehaviour{
    
	#region functionality variables
	
	private Transform main, reloadButton, unsentButton, newButton, viewAllButton;
	
	// New Training Report Form Variables
	private Transform newReportForm;
	private Dropdown trainerDropdown, traineeDropdown, siteDropdown;
	
	public string date, dateReport, trainer, trainee, site = "";
	public string correctSigns,	correctTaperLength,	correctTaperSpacing, correctSiteSpacing, correctSafetyZone, correctFitForPurpose = "";
	
	private GameObject newformSubmitButton;
	
	// View All Training Reports Variables
	private Transform viewReports;
	private string viewReportsBranch = "All";
	
	// View Detailed Training Report Variables
	private Transform report;
	
	// Error Variables
	private Transform error;
	private Text errorMessage;
	
	// Classes
	public class Trainer{
		public string name, branch;
		public Trainer(string newName, string newBranch){ name = newName; branch = newBranch; }
	}
	public List<Trainer> trainers = new List<Trainer>();
	
	public class Trainee{
		public string name, branch;
		public Trainee(string newName, string newBranch){ name = newName; branch = newBranch; }
	}
	public List<Trainee> trainees = new List<Trainee>();
	
	public class Report{
		
		public string date, dateReport, trainer, trainee, site, branch;
		public string correctSigns,	correctTaperLength,	correctTaperSpacing, correctSiteSpacing, correctSafetyZone, correctFitForPurpose;
		
		public Report(	
			string newDate, string newDateReport, string newTrainer, string newTrainee, string newSite, string newBranch,
			string newCorrectSigns,	string newCorrectTaperLength, string newCorrectTaperSpacing,
			string newCorrectSiteSpacing, string newCorrectSafetyZone, string newCorrectFitForPurpose
		){
			date = newDate;	dateReport = newDateReport; trainer = newTrainer;	trainee = newTrainee;	site = newSite;	branch = newBranch;
			correctSigns = newCorrectSigns;				correctTaperLength = newCorrectTaperLength;	correctTaperSpacing = newCorrectTaperSpacing;
			correctSiteSpacing = newCorrectSiteSpacing;	correctSafetyZone = newCorrectSafetyZone;	correctFitForPurpose  = newCorrectFitForPurpose;
		}
	}
	
	public List<Report> allReports = new List<Report>();	public List<Report> activeReports = new List<Report>();
	public List<Report> unsentReports = new List<Report>();
	
	#endregion functionality variables
	
	#region UI variables
	
	//General UI variables
	private Color darkBlue = new Color32(0, 204, 215, 255);		private Color lightBlue = new Color32(144, 227, 232, 255);
	private Color darkWarning = new Color32(215, 82, 0, 255);	private Color lightWarning = new Color32(232, 164, 144, 255);
	
	// New Training Report Form UI variables
	private Image trainer_Image;		private Image trainee_Image;		private Image site_Image;	
	private Image correctsigns_Image;	private Image correcttaper_Image;	private Image taperspacing_Image;	
	private Image sitespacing_Image;	private Image safetyzone_Image;		private Image fitforpurpose_Image;
	
	#endregion UI variables
	
	// Start is called before the first frame update
    void Start(){
		
		main = GameObject.Find("Main").transform;
		
		newButton = GameObject.Find("NewReportButton").transform;
		viewAllButton = GameObject.Find("ViewReportsButton").transform;
		reloadButton = GameObject.Find("UpdateButton").transform;
		unsentButton = GameObject.Find("SendUnsentButton").transform;
		
		newButton.localScale = viewAllButton.localScale = reloadButton.localScale = new Vector2(1, 1);
		unsentButton.localScale = new Vector2(0, 0);
		
		SetDate();
		
		// Assigning New Training Report Form Variables
		newReportForm = GameObject.Find("NewReportForm").transform; newReportForm.localScale = new Vector2(0, 0);
	  	trainerDropdown = GameObject.Find("Trainer_Dropdown").GetComponent<Dropdown>();	
		traineeDropdown = GameObject.Find("Trainee_Dropdown").GetComponent<Dropdown>();	
		siteDropdown = GameObject.Find("Site_Dropdown").GetComponent<Dropdown>();
		
		newformSubmitButton = GameObject.Find("SubmitNewForm");
		
		// Assigning View All Training Reports Variables
		viewReports = GameObject.Find("ViewReports").transform;		viewReports.localScale = new Vector2(0, 0);
		
		// Assigning View Detailed Training Report Variables
		report = GameObject.Find("Report").transform;				report.localScale = new Vector2(0, 0);
				
		// Assigning New Training Report Form UI variables
		trainer_Image = GameObject.Find("Trainer").GetComponent<Image>();				trainee_Image = GameObject.Find("Trainee").GetComponent<Image>();				site_Image = GameObject.Find("Site").GetComponent<Image>();						
		correctsigns_Image = GameObject.Find("CorrectSigns").GetComponent<Image>();		correcttaper_Image = GameObject.Find("CorrectTaper").GetComponent<Image>();
		taperspacing_Image = GameObject.Find("TaperSpacing").GetComponent<Image>();		sitespacing_Image = GameObject.Find("SiteSpacing").GetComponent<Image>();
		safetyzone_Image = GameObject.Find("SafetyZone").GetComponent<Image>();			fitforpurpose_Image = GameObject.Find("FitForPurpose").GetComponent<Image>();
  
		// Error Variables
		error = GameObject.Find("Error").transform; 				error.localScale = new Vector2(0, 0);
		errorMessage = GameObject.Find("ErrorMessage").GetComponent<Text>();
		
		StartCoroutine(FindDatabase());
	}

	void FixedUpdate(){
		
		if(unsentReports.Count > 0 && unsentButton.localScale.x == 0){
			unsentButton.localScale = new Vector2(1, 1);
		}
	}

	public void UpdateApp(){
		trainees.Clear(); trainers.Clear();	allReports.Clear();
		StartCoroutine(FindDatabase());
	}
	
	public void SendUnsentReports(){
		StartCoroutine(SendReports());
	}
	
	public IEnumerator SendReports(){
		
		// This function tests internet connectivity to the database before uploading information.
		// Checking put causes an error: Curl error 56: Receiving data failed with unitytls error code 1048578
		UnityWebRequest www = UnityWebRequest.Get("https://ttmtrainingapp-default-rtdb.firebaseio.com/Wilsons.json");
		using (www){ yield return www.SendWebRequest();
			
			// If there is an error connecting to the database
			if (www.isNetworkError || www.isHttpError){ 
				OpenErrorMessage("SendForm");
			}
			else {
			
				int i = 0;
				while(i < unsentReports.Count){
					string json = 	"{" +
										$"\"Date\":\"{unsentReports[i].date}\",\"Trainer\":\"{unsentReports[i].trainer}\",\"Trainee\":\"{unsentReports[i].trainee}\",\"Site\":\"{unsentReports[i].site}\"," +
										$"\"Correct Signs\":\"{unsentReports[i].correctSigns}\",\"Correct Taper Length\":\"{unsentReports[i].correctTaperLength}\"," +
										$"\"Correct Cone Spacing (Taper)\":\"{unsentReports[i].correctTaperSpacing}\",\"Correct Cone Spacing (Site)\":\"{unsentReports[i].correctSiteSpacing}\"," +
										$"\"Correct Safety Zone\":\"{unsentReports[i].correctSafetyZone}\",\"Site Fit For Purpose\":\"{unsentReports[i].correctFitForPurpose}\"" +
									"}";
										
					byte[] data = System.Text.Encoding.UTF8.GetBytes(json);
					UnityWebRequest put = UnityWebRequest.Put($"https://ttmtrainingapp-default-rtdb.firebaseio.com/Wilsons/Reports/{unsentReports[i].branch}/{dateReport}_{trainer}_{trainee}_{site}.json", data);
					yield return put.SendWebRequest();
					
					if (put.isNetworkError || put.isHttpError){ 
						break;
						yield return null;
					}
					
					i++;
				}
				
				unsentReports.Clear();
				unsentButton.localScale = new Vector2(0, 0);
			}
		}
	}
	
	// This function tests internet connectivity to the database before downloading information.
	public IEnumerator FindDatabase(){
		UnityWebRequest www = UnityWebRequest.Get("https://ttmtrainingapp-default-rtdb.firebaseio.com/Wilsons.json");
		using (www){ yield return www.SendWebRequest();
			
			// If there is an error connecting to the database
			if (www.isNetworkError || www.isHttpError){ 
				OpenErrorMessage("FindDatabase");
				
				GameObject.Find("NewReportButton").transform.localScale = new Vector2(0, 0);
				GameObject.Find("ViewReportsButton").transform.localScale = new Vector2(0, 0);
				reloadButton.position = new Vector2(reloadButton.position.x, main.position.y - 170f);
				unsentButton.position = new Vector2(unsentButton.position.x, main.position.y - 305f);
				
				yield return null;
			}
			else {
				
				GameObject.Find("NewReportButton").transform.localScale = new Vector2(1, 1);
				GameObject.Find("ViewReportsButton").transform.localScale = new Vector2(1, 1);
				reloadButton.position = new Vector2(reloadButton.position.x, main.position.y - 440f);
				unsentButton.position = new Vector2(unsentButton.position.x, main.position.y - 575f);
				
				StartCoroutine(FindStaffData());
				StartCoroutine(FindReports());
			}
		}
	}
	
	// This function retrives all of the data relating to staff from the database.
	public IEnumerator FindStaffData(){
		
		UnityWebRequest www = UnityWebRequest.Get("https://ttmtrainingapp-default-rtdb.firebaseio.com/Wilsons/Staff.json");
		using (www){ yield return www.SendWebRequest();
			
			var all = JSON.Parse(www.downloadHandler.text);
			int i = 1;

			while(i <= all.Count){ 
				string key = "Staff_0"; if(i >= 10){ key = "Staff_"; }
				
				trainees.Add(new Trainee(all[key+i]["Name"].Value, all[key+i]["Branch"].Value));
				
				if(bool.Parse(all[key+i]["Trainer"].Value)){ trainers.Add(new Trainer(all[key+i]["Name"].Value, all[key+i]["Branch"].Value)); }
				
				i++;
			}
				
			trainees = trainees.OrderBy(go=>go.name).ToList();	
			foreach(var x in trainees){	traineeDropdown.AddOptions( new List<string>{x.name});	}
						
			trainers = trainers.OrderBy(go=>go.name).ToList();
			foreach(var x in trainers){	trainerDropdown.AddOptions( new List<string>{x.name});	}
			
			GameObject.Find("Trainee_Label").GetComponent<Text>().text = GameObject.Find("Trainer_Label").GetComponent<Text>().text = GameObject.Find("Site_Label").GetComponent<Text>().text = "";
			
			RectTransform trainerTemplate = GameObject.Find("Trainer_Dropdown").transform.Find("Template").GetComponent<RectTransform>();
			trainerTemplate.sizeDelta = new Vector2(trainerTemplate.sizeDelta.x, trainers.Count * 120f);
			
			RectTransform traineeTemplate = GameObject.Find("Trainee_Dropdown").transform.Find("Template").GetComponent<RectTransform>();
			traineeTemplate.sizeDelta = new Vector2(traineeTemplate.sizeDelta.x, trainees.Count * 120f);
		}
	}
	
	// This function retrives all of the data relating to previous training reports from the database.	
	public IEnumerator FindReports(){
		UnityWebRequest www = UnityWebRequest.Get("https://ttmtrainingapp-default-rtdb.firebaseio.com/Wilsons/Reports.json");
		using (www){ yield return www.SendWebRequest();
			
			var all = JSON.Parse(www.downloadHandler.text);
							
			JSONNode.KeyEnumerator keysNelson = all["Nelson"].Keys;
			while(keysNelson.MoveNext()){ 
				allReports.Add(new Report(	
					all["Nelson"][keysNelson.Current]["Date"].Value,
					"",
					all["Nelson"][keysNelson.Current]["Trainer"].Value,
					all["Nelson"][keysNelson.Current]["Trainee"].Value,
					all["Nelson"][keysNelson.Current]["Site"].Value,
					"Nelson",
					all["Nelson"][keysNelson.Current]["Correct Signs"].Value,
					all["Nelson"][keysNelson.Current]["Correct Taper Length"].Value,
					all["Nelson"][keysNelson.Current]["Correct Cone Spacing (Taper)"].Value,
					all["Nelson"][keysNelson.Current]["Correct Cone Spacing (Site)"].Value,
					all["Nelson"][keysNelson.Current]["Correct Safety Zone"].Value,
					all["Nelson"][keysNelson.Current]["Site Fit For Purpose"].Value
				));	
			}
			
			JSONNode.KeyEnumerator keysChch = all["Christchurch"].Keys;
			while(keysChch.MoveNext()){  
				allReports.Add(new Report(
					all["Christchurch"][keysChch.Current]["Date"].Value,
					"",
					all["Christchurch"][keysChch.Current]["Trainer"].Value,
					all["Christchurch"][keysChch.Current]["Trainee"].Value,
					all["Christchurch"][keysChch.Current]["Site"].Value,
					"Christchurch",
					all["Christchurch"][keysChch.Current]["Correct Signs"].Value,
					all["Christchurch"][keysChch.Current]["Correct Taper Length"].Value,
					all["Christchurch"][keysChch.Current]["Correct Cone Spacing (Taper)"].Value,
					all["Christchurch"][keysChch.Current]["Correct Cone Spacing (Site)"].Value,
					all["Christchurch"][keysChch.Current]["Correct Safety Zone"].Value,
					all["Christchurch"][keysChch.Current]["Site Fit For Purpose"].Value
				));	  
			}
		}
	}
	
	
	public void OpenErrorMessage(string type){

		error.localScale = new Vector2(1, 1);
		
		if(type == "FindDatabase"){
			errorMessage.text = "You couldn't connect to the database, the application will have limited functionality.\n\nYou can load previous reports from the database from the Home screen when you reconnect to the Internet.";
		}
		else if(type == "SendForm"){
			errorMessage.text = "The Training report couldn't be sent to the database, it has been saved to your device instead.\n\n You can send the report to the database from the Home screen when you reconnect to the Internet.";
		}
	}
	
	public void CloseErrorMessage(){
		error.localScale = new Vector2(0, 0);
	}
	
	#region New Report Forms
	
	public void CreateNewForm(){
		newReportForm.localScale = new Vector2(1, 1);
			
		date = dateReport = trainer = trainee = site = "";
		correctSigns = correctTaperLength = correctTaperSpacing = correctSiteSpacing = correctSafetyZone = correctFitForPurpose = "";
				
		// This adds a blank option at the end of the dropdown list that is selected and then removed. 
		// This prevents the first option in the lists from being unselectable, as the original value doesn't change
		trainerDropdown.options.Add(new Dropdown.OptionData(){ text = "" });
		trainerDropdown.value = trainerDropdown.options.Count - 1;
		trainerDropdown.options.RemoveAt(trainerDropdown.options.Count - 1);
		
		traineeDropdown.options.Add(new Dropdown.OptionData(){ text = "" });
		traineeDropdown.value = traineeDropdown.options.Count - 1;
		traineeDropdown.options.RemoveAt(traineeDropdown.options.Count - 1);
		
		GameObject.Find("Trainee_Label").GetComponent<Text>().text = GameObject.Find("Trainer_Label").GetComponent<Text>().text = GameObject.Find("Site_Label").GetComponent<Text>().text = "";
		
		GameObject.Find("CorrectSigns_Yes").GetComponent<Image>().color =  GameObject.Find("CorrectSigns_No").GetComponent<Image>().color =	 GameObject.Find("CorrectSigns_NA").GetComponent<Image>().color = 		
		GameObject.Find("CorrectTaper_Yes").GetComponent<Image>().color =  GameObject.Find("CorrectTaper_No").GetComponent<Image>().color =  GameObject.Find("CorrectTaper_NA").GetComponent<Image>().color = 		
		GameObject.Find("TaperSpacing_Yes").GetComponent<Image>().color =  GameObject.Find("TaperSpacing_No").GetComponent<Image>().color =	 GameObject.Find("TaperSpacing_NA").GetComponent<Image>().color = 		
		GameObject.Find("SiteSpacing_Yes").GetComponent<Image>().color =   GameObject.Find("SiteSpacing_No").GetComponent<Image>().color =	 GameObject.Find("SiteSpacing_NA").GetComponent<Image>().color = 		
		GameObject.Find("SafetyZone_Yes").GetComponent<Image>().color =    GameObject.Find("SafetyZone_No").GetComponent<Image>().color =	 GameObject.Find("SafetyZone_NA").GetComponent<Image>().color =
		GameObject.Find("FitForPurpose_Yes").GetComponent<Image>().color = GameObject.Find("FitForPurpose_No").GetComponent<Image>().color = GameObject.Find("FitForPurpose_NA").GetComponent<Image>().color = new Color32(137, 137, 137, 255);
		
		newformSubmitButton.transform.Find("Text").GetComponent<Text>().text = "Submit";
		newformSubmitButton.GetComponent<Button>().interactable = true;
		
		SetDate();
	}
		
	public void SetDate(){
		System.DateTime now = System.DateTime.Now; 
		date = now.Day + "/" + now.Month + "/" + now.Year;
		dateReport = $"{now.Day}-{now.Month}-{now.Year}";
		GameObject.Find("Date_Text").GetComponent<Text>().text = date;
	}
	
	public void SetTrainer(){
		trainer = trainerDropdown.options[trainerDropdown.value].text;
	}
	
	public void SetTrainee(){
		trainee = traineeDropdown.options[traineeDropdown.value].text;
	}
	
	public void SetSite(){
		site = siteDropdown.options[siteDropdown.value].text;
	}
	
	public void CorrectSigns(string selected){
		
		correctSigns = selected;
		
		GameObject.Find("CorrectSigns_Yes").GetComponent<Image>().color = GameObject.Find("CorrectSigns_No").GetComponent<Image>().color =
		GameObject.Find("CorrectSigns_NA").GetComponent<Image>().color = new Color32(137, 137, 137, 255);
		
		GameObject.Find($"CorrectSigns_{selected}").GetComponent<Image>().color = new Color32(200, 200, 200, 255);
	}
	
	public void CorrectTaperLength(string selected){
		correctTaperLength = selected;
		
		GameObject.Find("CorrectTaper_Yes").GetComponent<Image>().color = GameObject.Find("CorrectTaper_No").GetComponent<Image>().color =
		GameObject.Find("CorrectTaper_NA").GetComponent<Image>().color = new Color32(137, 137, 137, 255);
		
		GameObject.Find($"CorrectTaper_{selected}").GetComponent<Image>().color = new Color32(200, 200, 200, 255);
	}
	
	public void CorrectTaperSpacing(string selected){
		correctTaperSpacing = selected;
		
		GameObject.Find("TaperSpacing_Yes").GetComponent<Image>().color = GameObject.Find("TaperSpacing_No").GetComponent<Image>().color =
		GameObject.Find("TaperSpacing_NA").GetComponent<Image>().color = new Color32(137, 137, 137, 255);
		
		GameObject.Find($"TaperSpacing_{selected}").GetComponent<Image>().color = new Color32(200, 200, 200, 255);
	}
	
	public void CorrectSiteSpacing(string selected){
		correctSiteSpacing = selected;
		
		GameObject.Find("SiteSpacing_Yes").GetComponent<Image>().color = GameObject.Find("SiteSpacing_No").GetComponent<Image>().color =
		GameObject.Find("SiteSpacing_NA").GetComponent<Image>().color = new Color32(137, 137, 137, 255);
		
		GameObject.Find($"SiteSpacing_{selected}").GetComponent<Image>().color = new Color32(200, 200, 200, 255);
	}
	
	public void CorrectSafetyZone(string selected){
		correctSafetyZone = selected;
		
		GameObject.Find("SafetyZone_Yes").GetComponent<Image>().color = GameObject.Find("SafetyZone_No").GetComponent<Image>().color =
		GameObject.Find("SafetyZone_NA").GetComponent<Image>().color = new Color32(137, 137, 137, 255);
		
		GameObject.Find($"SafetyZone_{selected}").GetComponent<Image>().color = new Color32(200, 200, 200, 255);
	}
	
	public void CorrectFitForPurpose(string selected){
		correctFitForPurpose = selected;
		
		GameObject.Find("FitForPurpose_Yes").GetComponent<Image>().color = GameObject.Find("FitForPurpose_No").GetComponent<Image>().color =
		GameObject.Find("FitForPurpose_NA").GetComponent<Image>().color = new Color32(137, 137, 137, 255);
		
		GameObject.Find($"FitForPurpose_{selected}").GetComponent<Image>().color = new Color32(200, 200, 200, 255);
	}

	public void Cancel(){
				
		trainer_Image.color = trainee_Image.color = site_Image.color =
		correctsigns_Image.color = correcttaper_Image.color = taperspacing_Image.color = 
		sitespacing_Image.color = safetyzone_Image.color = fitforpurpose_Image.color = lightBlue;
		
		newReportForm.localScale = new Vector2(0, 0);
	}

	public void Submit(){
		
		System.DateTime dateTime = System.DateTime.Parse(date);
		Debug.Log(dateTime);
		
		trainer_Image.color = trainee_Image.color = site_Image.color =
		correctsigns_Image.color = correcttaper_Image.color = taperspacing_Image.color = 
		sitespacing_Image.color = safetyzone_Image.color = fitforpurpose_Image.color = lightBlue;
		
		if(	trainer == "" || trainee == "" || trainer == trainee || site == "" || 
			correctSigns == "" ||correctTaperLength == "" || correctTaperSpacing == "" ||
			correctSiteSpacing == "" || correctSafetyZone == "" || correctFitForPurpose == ""){
			
			if(trainer == ""){						trainer_Image.color = lightWarning;			}															
			if(trainee == "" || trainer == trainee){trainee_Image.color = lightWarning; 		}																
			if(site == ""){							site_Image.color = lightWarning;			}
			if(correctSigns == ""){					correctsigns_Image.color = lightWarning;	}
			if(correctTaperLength == ""){			correcttaper_Image.color = lightWarning;	}
			if(correctTaperSpacing == ""){			taperspacing_Image.color = lightWarning;	}
			if(correctSiteSpacing == ""){			sitespacing_Image.color = lightWarning;		}
			if(correctSafetyZone == ""){			safetyzone_Image.color = lightWarning;		}
			if(correctFitForPurpose == ""){			fitforpurpose_Image.color = lightWarning;	}
			
		}
		else {
			string branch = trainers[trainers.FindIndex(x => x.name == trainer)].branch;
			allReports.Add(new Report(date, dateReport, trainer, trainee, site, branch, correctSigns, correctTaperLength, correctTaperSpacing, correctSiteSpacing, correctSafetyZone, correctFitForPurpose));
			newformSubmitButton.transform.Find("Text").GetComponent<Text>().text = "Sending";
			newformSubmitButton.GetComponent<Button>().interactable = false;
			StartCoroutine(SendForm(branch));
			
		}
	}

	public IEnumerator SendForm(string branch){
		
		// This function tests internet connectivity to the database before uploading information.
		// Checking put causes an error: Curl error 56: Receiving data failed with unitytls error code 1048578
		UnityWebRequest www = UnityWebRequest.Get("https://ttmtrainingapp-default-rtdb.firebaseio.com/Wilsons.json");
		using (www){ yield return www.SendWebRequest();
			
			// If there is an error connecting to the database
			if (www.isNetworkError || www.isHttpError){ 
				
				unsentReports.Add(new Report(date, dateReport, trainer, trainee, site, branch, correctSigns, correctTaperLength, correctTaperSpacing, correctSiteSpacing, correctSafetyZone, correctFitForPurpose));
				OpenErrorMessage("SendForm");
			}
			else {
				string json = 	"{" +
									$"\"Date\":\"{date}\",\"Trainer\":\"{trainer}\",\"Trainee\":\"{trainee}\",\"Site\":\"{site}\"," +
									$"\"Correct Signs\":\"{correctSigns}\",\"Correct Taper Length\":\"{correctTaperLength}\"," +
									$"\"Correct Cone Spacing (Taper)\":\"{correctTaperSpacing}\",\"Correct Cone Spacing (Site)\":\"{correctSiteSpacing}\"," +
									$"\"Correct Safety Zone\":\"{correctSafetyZone}\",\"Site Fit For Purpose\":\"{correctFitForPurpose}\"" +
								"}";
							
				byte[] data = System.Text.Encoding.UTF8.GetBytes(json);
				UnityWebRequest put = UnityWebRequest.Put($"https://ttmtrainingapp-default-rtdb.firebaseio.com/Wilsons/Reports/{branch}/{dateReport}_{trainer}_{trainee}_{site}.json", data);
				yield return put.SendWebRequest();
			}
			newReportForm.localScale = new Vector2(0, 0);
		}			
	}

	#endregion New Report Forms

	#region View All Reports 
	
	public void OpenReports(){
		viewReports.localScale = new Vector2(1, 1);
		CreateSmallReports();
	}
	
	public void ChangeBranch(){
		
		viewReportsBranch = GameObject.Find("Branch_Label").GetComponent<Text>().text;
		CreateSmallReports();
	}
	
	public void CreateSmallReports(){
		
		int i = 0;
		while(i < activeReports.Count){
			Destroy(GameObject.Find($"ReportSmall_{i}"));
			i++;
		}
		
		activeReports.Clear();
				
		i = 0;
		if(viewReportsBranch == "All"){ 
			while(i < allReports.Count){
				activeReports.Add( new Report(
					allReports[i].date,	allReports[i].dateReport, allReports[i].trainer, allReports[i].trainee, allReports[i].site, allReports[i].branch,
					allReports[i].correctSigns,			allReports[i].correctTaperLength,	allReports[i].correctTaperSpacing,
					allReports[i].correctSiteSpacing,	allReports[i].correctSafetyZone,	allReports[i].correctFitForPurpose
				));
				i++;
			}
		}
		else {
			while(i < allReports.Count){
				if(allReports[i].branch == viewReportsBranch){
					activeReports.Add( new Report(
						allReports[i].date,	allReports[i].dateReport, allReports[i].trainer, allReports[i].trainee, allReports[i].site, allReports[i].branch,
						allReports[i].correctSigns,			allReports[i].correctTaperLength,	allReports[i].correctTaperSpacing,
						allReports[i].correctSiteSpacing,	allReports[i].correctSafetyZone,	allReports[i].correctFitForPurpose
					));
				}
				i++;
			}
		}
		
		int j = 0;
		
		RectTransform parent_Rect = GameObject.Find("Reports").GetComponent<RectTransform>();
		parent_Rect.sizeDelta = new Vector2(parent_Rect.sizeDelta.x, Screen.height - 500f);
		
		RectTransform scroll_Rect = GameObject.Find("Reports_Scroll").transform.GetComponent<RectTransform>();
		
		scroll_Rect.sizeDelta = new Vector2(scroll_Rect.sizeDelta.x, (180f * activeReports.Count) - 10f);
		// The -1f prevents the scroll from bouncing downwards 500px or so
		scroll_Rect.position = new Vector2(scroll_Rect.position.x, parent_Rect.position.y - 1f);
		
		Debug.Log(scroll_Rect.position);
		if(scroll_Rect.sizeDelta.y < parent_Rect.sizeDelta.y){	parent_Rect.GetComponent<ScrollRect>().enabled = false; }
		else{													parent_Rect.GetComponent<ScrollRect>().enabled = true;	 }
		
		while(j < activeReports.Count){

			GameObject newSmallReport = Instantiate(Resources.Load<GameObject>("Prefabs/ReportSmall"), new Vector2(scroll_Rect.transform.position.x, (scroll_Rect.transform.position.y - 85f) - (180 * j)), Quaternion.identity, scroll_Rect.transform);
			newSmallReport.name = $"ReportSmall_{j}";
			newSmallReport.GetComponent<Button>().onClick.AddListener(delegate{ OpenReport(); });
			
			newSmallReport.transform.Find("Top").transform.Find("Date").GetComponent<Text>().text = activeReports[j].date;
			newSmallReport.transform.Find("Top").transform.Find("Trainer").GetComponent<Text>().text = activeReports[j].trainer;
			newSmallReport.transform.Find("Bottom").transform.Find("Trainee").GetComponent<Text>().text = activeReports[j].trainee;
			newSmallReport.transform.Find("Bottom").transform.Find("Site").GetComponent<Text>().text = activeReports[j].site;
			
			j++;
		}
	}
	
	public void Back(){
		viewReports.localScale = new Vector2(0, 0);
	}
	
	#endregion View All Reports
	
	#region View Detailed Report
		public void OpenReport(){
			
			string buttonPressed = EventSystem.current.currentSelectedGameObject.name;
			int index = int.Parse(buttonPressed.Substring(12, buttonPressed.Length - 12));
						
			viewReports.localScale = new Vector2(0, 0);
			report.localScale = new Vector2(1, 1);
						
			GameObject.Find("Report_DateText").GetComponent<Text>().text = activeReports[index].date;			GameObject.Find("Report_TrainerText").GetComponent<Text>().text = activeReports[index].trainer;
			GameObject.Find("Report_TraineeText").GetComponent<Text>().text = activeReports[index].trainee;		GameObject.Find("Report_SiteText").GetComponent<Text>().text = activeReports[index].site;
		
			string viewCorrectSigns = activeReports[index].correctSigns;			string viewTaperLength = activeReports[index].correctTaperLength;
			string viewTaperSpacing = activeReports[index].correctTaperSpacing;		string viewSiteSpacing = activeReports[index].correctSiteSpacing;
			string viewSafetyZone = activeReports[index].correctSafetyZone;			string viewFitForPurpose = activeReports[index].correctFitForPurpose;
			
			if(viewCorrectSigns == "NA"){	viewCorrectSigns = "N/A";	}			if(viewTaperLength == "NA")  {	viewTaperLength = "N/A";	}	
			if(viewTaperSpacing == "NA"){	viewTaperSpacing = "N/A";	}			if(viewSiteSpacing == "NA")  {	viewSiteSpacing = "N/A";	}	
			if(viewSafetyZone == "NA")  {	viewSafetyZone = "N/A";		}			if(viewFitForPurpose == "NA"){	viewFitForPurpose = "N/A";	}
				
			GameObject.Find("Report_CorrectSignsText").GetComponent<Text>().text = viewCorrectSigns;	GameObject.Find("Report_CorrectTaperText").GetComponent<Text>().text = viewTaperLength;
			GameObject.Find("Report_TaperSpacingText").GetComponent<Text>().text = viewTaperSpacing;	GameObject.Find("Report_SiteSpacingText").GetComponent<Text>().text = viewSiteSpacing;
			GameObject.Find("Report_SafetyZoneText").GetComponent<Text>().text = viewSafetyZone;		GameObject.Find("Report_FitForPurposeText").GetComponent<Text>().text = viewFitForPurpose;
			
		}
	
		public void CloseReport(){
			report.localScale = new Vector2(0, 0);
			OpenReports();
		
		}
	#endregion View Detailed Report
}
