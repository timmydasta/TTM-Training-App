using System.Collections; using System.Collections.Generic; using UnityEngine; using UnityEngine.UI;
using UnityEngine.Networking; using SimpleJSON; using System.Linq;

public class MainScript : MonoBehaviour{
    
	public string date = "";
	public string dateReport = "";
	public string trainer = "";
	public string trainee = "";
	public string site = "";
	
	public string correctSigns = "";
	public string correctTaperLength = "";
	public string correctTaperSpacing = "";
	public string correctSiteSpacing = "";
	public string correctSafetyZone = "";
	public string correctFitForPurpose = "";
	
	private Transform reportForm;
	private Dropdown trainerDropdown;
	private Dropdown traineeDropdown;
	private Dropdown siteDropdown;
	
	public class Trainer{
		public string name;
		public Trainer(string newName){ name = newName; }
	}
	public List<Trainer> trainers = new List<Trainer>();
	
	public class Trainee{
		public string name;
		public Trainee(string newName){ name = newName; }
	}
	public List<Trainee> trainees = new List<Trainee>();
	
	// Start is called before the first frame update
    void Start(){
		
		SetDate();
		
		reportForm = GameObject.Find("NewReportForm").transform;
	  	trainerDropdown = GameObject.Find("Trainer_Dropdown").GetComponent<Dropdown>();	traineeDropdown = GameObject.Find("Trainee_Dropdown").GetComponent<Dropdown>();	siteDropdown = GameObject.Find("Site_Dropdown").GetComponent<Dropdown>();
		GameObject.Find("Trainee_Label").GetComponent<Text>().text = GameObject.Find("Trainer_Label").GetComponent<Text>().text = GameObject.Find("Site_Label").GetComponent<Text>().text = "";
		
		StartCoroutine(FindData());
    }

    // Update is called once per frame
    void Update(){
    }
		
	public void createNewForm(){
		reportForm.localScale = new Vector2(1, 1);
			
		date = dateReport = trainer = trainee = site = "";
		correctSigns = correctTaperLength = correctTaperSpacing = correctSiteSpacing = correctSafetyZone = correctFitForPurpose = "";
		
		GameObject.Find("Trainee_Label").GetComponent<Text>().text = GameObject.Find("Trainer_Label").GetComponent<Text>().text = GameObject.Find("Site_Label").GetComponent<Text>().text = "";
		
		GameObject.Find("CorrectSigns_Yes").GetComponent<Image>().color =  GameObject.Find("CorrectSigns_No").GetComponent<Image>().color =	 GameObject.Find("CorrectSigns_NA").GetComponent<Image>().color = 		
		GameObject.Find("CorrectTaper_Yes").GetComponent<Image>().color =  GameObject.Find("CorrectTaper_No").GetComponent<Image>().color =  GameObject.Find("CorrectTaper_NA").GetComponent<Image>().color = 		
		GameObject.Find("TaperSpacing_Yes").GetComponent<Image>().color =  GameObject.Find("TaperSpacing_No").GetComponent<Image>().color =	 GameObject.Find("TaperSpacing_NA").GetComponent<Image>().color = 		
		GameObject.Find("SiteSpacing_Yes").GetComponent<Image>().color =   GameObject.Find("SiteSpacing_No").GetComponent<Image>().color =	 GameObject.Find("SiteSpacing_NA").GetComponent<Image>().color = 		
		GameObject.Find("SafetyZone_Yes").GetComponent<Image>().color =    GameObject.Find("SafetyZone_No").GetComponent<Image>().color =	 GameObject.Find("SafetyZone_NA").GetComponent<Image>().color =
		GameObject.Find("FitForPurpose_Yes").GetComponent<Image>().color = GameObject.Find("FitForPurpose_No").GetComponent<Image>().color = GameObject.Find("FitForPurpose_NA").GetComponent<Image>().color = new Color32(137, 137, 137, 255);
		
		SetDate();
	}
		
	public IEnumerator FindData(){
		
		UnityWebRequest www = UnityWebRequest.Get("https://ttmtrainingapp-default-rtdb.firebaseio.com/Wilsons.json");
		using (www){ yield return www.SendWebRequest();
			
			var all = JSON.Parse(www.downloadHandler.text);
			
			string key = "Staff_0"; int i = 1;
			if(i >= 10){ key = "Staff_"; }

			while(i <= all["Staff"].Count){
				
				if(i >= 10){ key = "Staff_"; }
				
				trainees.Add(new Trainee(all["Staff"][key+i]["Name"].Value));
				
				if(bool.Parse(all["Staff"][key+i]["Trainer"].Value)){
					trainers.Add(new Trainer(all["Staff"][key+i]["Name"].Value));
				}
				
				i++;
			}
				
			trainees = trainees.OrderBy(go=>go.name).ToList();	
			foreach(var x in trainees){
				traineeDropdown.AddOptions( new List<string>{x.name});
			}
						
			trainers = trainers.OrderBy(go=>go.name).ToList();
			foreach(var x in trainers){
				trainerDropdown.AddOptions( new List<string>{x.name});
			}
			
			GameObject.Find("Trainee_Label").GetComponent<Text>().text = GameObject.Find("Trainer_Label").GetComponent<Text>().text = GameObject.Find("Site_Label").GetComponent<Text>().text = "";
			
			RectTransform trainerTemplate = GameObject.Find("Trainer_Dropdown").transform.Find("Template").GetComponent<RectTransform>();
			trainerTemplate.sizeDelta = new Vector2(trainerTemplate.sizeDelta.x, trainers.Count * 80f);
			
			RectTransform traineeTemplate = GameObject.Find("Trainee_Dropdown").transform.Find("Template").GetComponent<RectTransform>();
			traineeTemplate.sizeDelta = new Vector2(traineeTemplate.sizeDelta.x, trainees.Count * 80f);
		}
	}

	public void SetDate(){
		System.DateTime now = System.DateTime.Now; 
		date = now.Day + "/" + now.Month + "/" + now.Year;
		dateReport = $"{now.Day}-{now.Month}-{now.Year}";
		GameObject.Find("Date_Input").GetComponent<InputField>().text = date;
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

	public void Submit(){
		
		GameObject.Find("Trainer").GetComponent<Image>().sprite = GameObject.Find("Trainee").GetComponent<Image>().sprite = Resources.Load<Sprite>("IMG/WilsonsSmallDropDown");
		GameObject.Find("Site").GetComponent<Image>().sprite = Resources.Load<Sprite>("IMG/WilsonsLargeDropDown");
		GameObject.Find("CorrectSigns").GetComponent<Image>().sprite = GameObject.Find("CorrectTaper").GetComponent<Image>().sprite = 
		GameObject.Find("TaperSpacing").GetComponent<Image>().sprite = GameObject.Find("SiteSpacing").GetComponent<Image>().sprite = 
		GameObject.Find("SafetyZone").GetComponent<Image>().sprite = GameObject.Find("FitForPurpose").GetComponent<Image>().sprite = Resources.Load<Sprite>("IMG/WilsonsLarge");
		
		if(trainer == "" || trainee == "" || trainer == trainee || site == "" || correctSigns == "" ||correctTaperLength == "" || correctTaperSpacing == "" || correctSiteSpacing == "" || correctSafetyZone == "" || correctFitForPurpose == ""){
			
			if(trainer == ""){							GameObject.Find("Trainer").GetComponent<Image>().sprite = Resources.Load<Sprite>("IMG/WilsonsSmallDropDown_Warning");	}
			if(trainee == "" || trainer == trainee){	GameObject.Find("Trainee").GetComponent<Image>().sprite = Resources.Load<Sprite>("IMG/WilsonsSmallDropDown_Warning");	}
			if(site == ""){								GameObject.Find("Site").GetComponent<Image>().sprite = Resources.Load<Sprite>("IMG/WilsonsLargeDropDown_Warning");		}
			if(correctSigns == ""){						GameObject.Find("CorrectSigns").GetComponent<Image>().sprite = Resources.Load<Sprite>("IMG/WilsonsLarge_Warning");		}
			if(correctTaperLength == ""){				GameObject.Find("CorrectTaper").GetComponent<Image>().sprite = Resources.Load<Sprite>("IMG/WilsonsLarge_Warning");		}
			if(correctTaperSpacing == ""){				GameObject.Find("TaperSpacing").GetComponent<Image>().sprite = Resources.Load<Sprite>("IMG/WilsonsLarge_Warning");		}
			if(correctSiteSpacing == ""){				GameObject.Find("SiteSpacing").GetComponent<Image>().sprite = Resources.Load<Sprite>("IMG/WilsonsLarge_Warning");		}
			if(correctSafetyZone == ""){				GameObject.Find("SafetyZone").GetComponent<Image>().sprite = Resources.Load<Sprite>("IMG/WilsonsLarge_Warning");		}
			if(correctFitForPurpose == ""){				GameObject.Find("FitForPurpose").GetComponent<Image>().sprite = Resources.Load<Sprite>("IMG/WilsonsLarge_Warning");		}
			
		}
		else {
			Debug.Log("All Correct");
			StartCoroutine(SendForm());
			reportForm.localScale = new Vector2(0, 0);
		}
	}

	public IEnumerator SendForm(){
		
		string json = 	"{" +
							$"\"Date\":\"{date}\",\"Trainer\":\"{trainer}\",\"Trainee\":\"{trainee}\",\"Site\":\"{site}\"," +
							$"\"Correct Signs\":\"{correctSigns}\",\"Correct Taper Length\":\"{correctTaperLength}\"," +
							$"\"Correct Cone Spacing (Taper)\":\"{correctTaperSpacing}\",\"Correct Cone Spacing (Site)\":\"{correctSiteSpacing}\"," +
							$"\"Correct Safety Zone\":\"{correctSafetyZone}\",\"Site Fit For Purpose\":\"{correctFitForPurpose}\"" +
						"}";
					
		byte[] data = System.Text.Encoding.UTF8.GetBytes(json);
		UnityWebRequest put = UnityWebRequest.Put($"https://ttmtrainingapp-default-rtdb.firebaseio.com/Wilsons/Reports/{dateReport}_{trainer}_{trainee}_{site}.json", data);
		yield return put.SendWebRequest();
						
	}
}
