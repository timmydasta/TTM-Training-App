using System.Collections; using System.Collections.Generic; using UnityEngine; using UnityEngine.UI;
using UnityEngine.Networking; using SimpleJSON; using System.Linq;

public class MainScript : MonoBehaviour{
    
	public string date = "";
	public string trainer = "";
	public string trainee = "";
	public string site = "";
	
	public string correctSigns = "";
	public string correctTaperLength = "";
	public string correctTaperSpacing = "";
	public string correctSiteSpacing = "";
	public string correctSafetyZone = "";
	public string correctFitForPurpose = "";
	
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
		
		System.DateTime now = System.DateTime.Now; 
		date = now.Day + "/" + now.Month + "/" + now.Year;
		GameObject.Find("Date_Input").GetComponent<InputField>().text = date;
	  
	  	trainerDropdown = GameObject.Find("Trainer_Dropdown").GetComponent<Dropdown>();
		traineeDropdown = GameObject.Find("Trainee_Dropdown").GetComponent<Dropdown>();
		siteDropdown = GameObject.Find("Site_Dropdown").GetComponent<Dropdown>();
	  
		GameObject.Find("Site_Label").GetComponent<Text>().text = "";
	   
		StartCoroutine(FindData());
    }

    // Update is called once per frame
    void Update(){
        
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
			GameObject.Find("Trainee_Label").GetComponent<Text>().text = "";
			
			trainers = trainers.OrderBy(go=>go.name).ToList();
			foreach(var x in trainers){
				trainerDropdown.AddOptions( new List<string>{x.name});
			}
			GameObject.Find("Trainer_Label").GetComponent<Text>().text = "";
			
			GameObject.Find("Site_Label").GetComponent<Text>().text = "";
		}
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
		
		string button = "CorrectSigns_";
		GameObject.Find(button + "Yes").GetComponent<Image>().color = GameObject.Find(button + "No").GetComponent<Image>().color =
		GameObject.Find(button + "NA").GetComponent<Image>().color = new Color32(137, 137, 137, 255);
		
		GameObject.Find(button + selected).GetComponent<Image>().color = new Color32(200, 200, 200, 255);
	}
	
	public void CorrectTaperLength(string selected){
		correctTaperLength = selected;
		
		string button = "CorrectTaper_";
		GameObject.Find(button + "Yes").GetComponent<Image>().color = GameObject.Find(button + "No").GetComponent<Image>().color =
		GameObject.Find(button + "NA").GetComponent<Image>().color = new Color32(137, 137, 137, 255);
		
		GameObject.Find(button + selected).GetComponent<Image>().color = new Color32(200, 200, 200, 255);
	}
	
	public void CorrectTaperSpacing(string selected){
		correctTaperSpacing = selected;
		
		string button = "TaperSpacing_";
		GameObject.Find(button + "Yes").GetComponent<Image>().color = GameObject.Find(button + "No").GetComponent<Image>().color =
		GameObject.Find(button + "NA").GetComponent<Image>().color = new Color32(137, 137, 137, 255);
		
		GameObject.Find(button + selected).GetComponent<Image>().color = new Color32(200, 200, 200, 255);
	}
	
	public void CorrectSiteSpacing(string selected){
		correctSiteSpacing = selected;
		
		string button = "SiteSpacing_";
		GameObject.Find(button + "Yes").GetComponent<Image>().color = GameObject.Find(button + "No").GetComponent<Image>().color =
		GameObject.Find(button + "NA").GetComponent<Image>().color = new Color32(137, 137, 137, 255);
		
		GameObject.Find(button + selected).GetComponent<Image>().color = new Color32(200, 200, 200, 255);
	}
	
	public void CorrectSafetyZone(string selected){
		correctSafetyZone = selected;
		
		string button = "SafetyZone_";
		GameObject.Find(button + "Yes").GetComponent<Image>().color = GameObject.Find(button + "No").GetComponent<Image>().color =
		GameObject.Find(button + "NA").GetComponent<Image>().color = new Color32(137, 137, 137, 255);
		
		GameObject.Find(button + selected).GetComponent<Image>().color = new Color32(200, 200, 200, 255);
	}
	
	public void CorrectFitForPurpose(string selected){
		correctFitForPurpose = selected;
		
		string button = "FitForPurpose_";
		GameObject.Find(button + "Yes").GetComponent<Image>().color = GameObject.Find(button + "No").GetComponent<Image>().color =
		GameObject.Find(button + "NA").GetComponent<Image>().color = new Color32(137, 137, 137, 255);
		
		GameObject.Find(button + selected).GetComponent<Image>().color = new Color32(200, 200, 200, 255);
	}

	public void Submit(){
		
		GameObject.Find("Trainer").GetComponent<Image>().sprite = GameObject.Find("Trainer").GetComponent<Image>().sprite = Resources.Load<Sprite>("IMG/WilsonsSmallDropDown");
		GameObject.Find("Site").GetComponent<Image>().sprite = Resources.Load<Sprite>("IMG/WilsonsLargeDropDown");
		
		GameObject.Find("CorrectSigns").GetComponent<Image>().sprite = GameObject.Find("CorrectTaper").GetComponent<Image>().sprite = 
		GameObject.Find("TaperSpacing").GetComponent<Image>().sprite = GameObject.Find("SiteSpacing").GetComponent<Image>().sprite = 
		GameObject.Find("SafetyZone").GetComponent<Image>().sprite = GameObject.Find("FitForPurpose").GetComponent<Image>().sprite = Resources.Load<Sprite>("IMG/WilsonsLarge");
		
		if(trainer == "" || trainee == "" || site == "" || correctSigns == "" ||correctTaperLength == "" || correctTaperSpacing == "" || correctSiteSpacing == "" || correctSafetyZone == "" || correctFitForPurpose == ""){
			
			if(trainer == ""){					GameObject.Find("Trainer").GetComponent<Image>().sprite = Resources.Load<Sprite>("IMG/WilsonsSmallDropDown_Warning");	}
			if(trainee == ""){				GameObject.Find("Trainee").GetComponent<Image>().sprite = Resources.Load<Sprite>("IMG/WilsonsSmallDropDown_Warning");	}
			if(site == ""){				GameObject.Find("Site").GetComponent<Image>().sprite = Resources.Load<Sprite>("IMG/WilsonsLargeDropDown_Warning");		}
			if(correctSigns == ""){		GameObject.Find("CorrectSigns").GetComponent<Image>().sprite = Resources.Load<Sprite>("IMG/WilsonsLarge_Warning");		}
			if(correctTaperLength == ""){	GameObject.Find("CorrectTaper").GetComponent<Image>().sprite = Resources.Load<Sprite>("IMG/WilsonsLarge_Warning");		}
			if(correctTaperSpacing == ""){	GameObject.Find("TaperSpacing").GetComponent<Image>().sprite = Resources.Load<Sprite>("IMG/WilsonsLarge_Warning");		}
			if(correctSiteSpacing == ""){	GameObject.Find("SiteSpacing").GetComponent<Image>().sprite = Resources.Load<Sprite>("IMG/WilsonsLarge_Warning");		}
			if(correctSafetyZone == ""){	GameObject.Find("SafetyZone").GetComponent<Image>().sprite = Resources.Load<Sprite>("IMG/WilsonsLarge_Warning");		}
			if(correctFitForPurpose == ""){GameObject.Find("FitForPurpose").GetComponent<Image>().sprite = Resources.Load<Sprite>("IMG/WilsonsLarge_Warning");		}
			
		}
		else {
			Debug.Log("All Correct");
		}
		
		
		Debug.Log(trainer + " " + trainee + " " + site);
	}

}
