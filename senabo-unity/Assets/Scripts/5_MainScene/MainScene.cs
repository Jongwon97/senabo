using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainScene : MonoBehaviour
{
    public GameObject actionModal, LocationAlertModalPanel, EmerPoopModalPanel, EmerBiteModalPanel;
    public Text LocationAlertModalText;
    public Text MainTitleText;
    public GameObject dogImage, imageBedPoop, imageVomit;
    public Image imageBed, imageCarpet, imageFlower, imagePoopPad, imageWaterBowl, imageFoodBowl;
    public Sprite bedClean, bedDirty, carpetClean, carpetDirty, flowerClean, flowerDirty, poopPadClean, poopPadDirty,
                fullWaterBowl, emptyWaterBowl, fullFoodBowl, emptyFoodBowl;
    public bool isPoop = false, emerPoop = false, emerStomachache = false, emerAnxiety = false, emerDepression = false,
                emerCrush = false, emerBite = false, emerWalk = false, emerBarking = false, emerVomiting = false;
    private Button dogBody;
    private bool actionable;

    [SerializeField]
    private GameObject LocationManagerObject;
    private MainLocationManager locationManager;

    public GameObject EndAlertModal;
    public Text EndAlertModalText;

    void Start()
    {
        isPoop = false;
        emerPoop = false;
        emerStomachache = false;
        emerAnxiety = false;
        emerDepression = false;
        emerCrush = false;
        emerBite = false;
        emerWalk = false;
        emerBarking = false;
        emerVomiting = false;
        actionable = true;

        actionModal.SetActive(false);
        EndAlertModal.SetActive(false);

        StartCoroutine(CheckIsPoop());
        StartCoroutine(CheckEmergency());

        LocationAlertModalText.text = $"{PlayerPrefs.GetString("dogName")}{GetGaVerb(PlayerPrefs.GetString("dogName"))}\n집에서 기다리고 있어요!";
        locationManager = LocationManagerObject.GetComponent<MainLocationManager>();

        EndAlertModalText.text = $"{PlayerPrefs.GetString("dogName")}{GetVerb(PlayerPrefs.GetString("dogName"))} 함께 할 수 있는\n8주간의 시간이 끝났어요.\n\n다시 시작하고 싶다면\n탈퇴 후 재가입해주세요.";

        SetTitleDayCount();
    }

    string GetVerb(string dogName)
    {
        char lastLetter = dogName.ElementAt(dogName.Length - 1);

        // 한글의 제일 처음과 끝의 범위 밖일 경우 
        if (lastLetter < 0xAC00 || lastLetter > 0xD7A3)
        {
            return "와";
        }

        return (lastLetter - 0xAC00) % 28 > 0 ? "이와" : "와";
    }

    string GetGaVerb(string dogName)
    {
        char lastLetter = dogName.ElementAt(dogName.Length - 1);

        // 한글의 제일 처음과 끝의 범위 밖일 경우 
        if (lastLetter < 0xAC00 || lastLetter > 0xD7A3)
        {
            return "가";
        }

        return (lastLetter - 0xAC00) % 28 > 0 ? "이가" : "가";
    }

    public void OnClickEmerPoop()
    {
        if (emerPoop) // Never Used
        {
            emerPoop = false;
            imageBedPoop.SetActive(false);
            EmerPoopModalPanel.SetActive(true);
            Invoke(nameof(CloseLocationAlertModal), 2.0f);
        }
    }

    public void OnClickEmerStomachache()
    {
        if (emerStomachache) // Never Used
        {
            emerStomachache = false;
            ReceiptScene.type = ReceiptType.HospitalCost3;
            StartCoroutine(UpdatePosition("MoveHospitalScene"));
        }
    }

    public void OnClickEmerAnxiety()
    {
        if (emerAnxiety)
        {
            emerAnxiety = false;
            // Other Loading Scene is needed!!!
            ReceiptScene.type = ReceiptType.DamageCost1;
            StartCoroutine(UpdatePosition("ReceiptScene"));
        }
    }

    public void OnClickEmerDepression()
    {
        if (emerDepression)
        {
            emerDepression = false;
            // Other Loading Scene is needed!!!
            ReceiptScene.type = ReceiptType.DamageCost2;
            StartCoroutine(UpdatePosition("ReceiptScene"));
        }
    }

    public void OnClickEmerCrush()
    {
        if (emerCrush)
        {
            emerCrush = false;
            // Other Loading Scene is needed!!!
            ReceiptScene.type = ReceiptType.DamageCost3;
            StartCoroutine(UpdatePosition("ReceiptScene"));
        }
    }

    public void OnClickEmerVomit()
    {
        if (emerVomiting)
        {
            emerVomiting = false;
            imageVomit.SetActive(false);
            ReceiptScene.type = ReceiptType.HospitalCost5;
            StartCoroutine(UpdatePosition("MoveHospitalScene"));
        }
    }

    void AfterJudgePoop()
    {
        Debug.Log("배변 여부: " + isPoop); // DEBUG CODE
        if (isPoop)
        {
            imagePoopPad.sprite = poopPadDirty;
        }
    }

    void AfterJudgeEmergency()
    {
        if (emerPoop) imageBedPoop.SetActive(true);
        else imageBedPoop.SetActive(false);

        if (emerStomachache)
        {
            imageWaterBowl.sprite = fullWaterBowl;
            imageFoodBowl.sprite = fullFoodBowl;
        }
        else
        {
            imageWaterBowl.sprite = emptyWaterBowl;
            imageFoodBowl.sprite = emptyFoodBowl;
        }

        if (emerAnxiety) // carpet, bed, flower random
        {
            imageCarpet.sprite = carpetDirty;
        }
        else
        {
            imageCarpet.sprite = carpetClean;
        }

        if (emerDepression)
        {
            imageBed.sprite = bedDirty;
        }
        else
        {
            imageBed.sprite = bedClean;
        }

        if (emerCrush)
        {
            imageFlower.sprite = flowerDirty;
        }
        else
        {
            imageFlower.sprite = flowerClean;
        }

        if (emerBite)
        {
            EmerBiteModalPanel.SetActive(true);
            Invoke(nameof(CloseLocationAlertModal), 2.0f);
        }

        if (emerWalk)
        {
            Debug.Log("산책 호소"); // DEBUG CODE
        }

        if (emerBarking)
        {
            Debug.Log("큰 소리로 짖음"); // DEBUG CODE
        }

        if (emerVomiting)
        {
            imageVomit.SetActive(true);
        }
        else
        {
            imageVomit.SetActive(false);
        }

    }

    void SetBooleanValue(string type, bool state)
    {
        Debug.Log(type + " 타입: " + state); // DEBUG CODE

        switch (type)
        {
            case "POOP":
                emerPoop = !state; break;
            case "STOMACHACHE":
                emerStomachache = !state; break;
            case "ANXIETY":
                emerAnxiety = !state; break;
            case "DEPRESSION":
                emerDepression = !state; break;
            case "CRUSH":
                emerCrush = !state; break;
            case "BITE":
                emerBite = !state; break;
            case "WALK":
                emerWalk = !state; break;
            case "BARKING":
                emerBarking = !state; break;
            case "VOMITING":
                emerVomiting = !state; break;
        }
    }

    IEnumerator CheckIsPoop()
    {
        string url = ServerSettings.SERVER_URL + "/api/feed/latest"; // TEST CODE

        UnityWebRequest www = UnityWebRequest.Get(url);

        string accessToken = PlayerPrefs.GetString("accessToken");
        string jwtToken = $"Bearer {accessToken}";
        www.SetRequestHeader("Authorization", jwtToken);

        Debug.Log("똥1" + isPoop);

        yield return www.SendWebRequest();

        if (www.error == null)
        {
            Debug.Log("MainScene CheckIsPoop Success"); // Debug Code

            Debug.Log("똥2" + isPoop);

            FeedLatestDtoClass poop = JsonUtility.FromJson<APIResponse<FeedLatestDtoClass>>(www.downloadHandler.text).data;

            DateTime poopTime = Convert.ToDateTime(poop.createTime).AddHours(1);
            Debug.Log("poopTime + 1: " + poopTime); // TEST
            Debug.Log("curTime: " + DateTime.Now); // TEST


            if (DateTime.Now >= poopTime && !poop.cleanYn)
            {
                isPoop = true;
                Debug.Log("시간이 됨, 똥 있음"); // TEST
            }
            else
            {
                Debug.Log("똥3" + isPoop);
                Debug.Log("똥 아직 없음"); // TEST}
            }
        }
        else
        {
            Debug.Log("MainScene CheckIsPoop Error:"); // Debug Code

            if (www.responseCode == 404)
            {
                isPoop = false;
            }
            else if (www.responseCode == 403)
            {
                RefreshTokenManager.Instance.ReIssueRefreshToken();
                StartCoroutine(CheckIsPoop());
            }
            else if (www.responseCode != 404)
            {
                Debug.Log("CheckIsPoop - 이미 메인화면이기 때문에 메인화면으로 돌아가는 방식 사용 불가!"); // NEED HELP
                SceneManager.LoadScene("MainScene");
            }
        }

        Debug.Log("MainScene의 배변 여부: " + isPoop);// TEST
        AfterJudgePoop();
    }

    IEnumerator CheckEmergency()
    {
        string url = ServerSettings.SERVER_URL + "/api/emergency/get";

        UnityWebRequest response = UnityWebRequest.Get(url);

        string accessToken = PlayerPrefs.GetString("accessToken");
        string jwtToken = $"Bearer {accessToken}";
        response.SetRequestHeader("Authorization", jwtToken);

        yield return response.SendWebRequest();

        if (response.error == null)
        {
            Debug.Log("MainScene CheckEmergency Success"); // Debug Code

            APIResponse<List<EmergencyDtoClass>> apiResponse
            = JsonUtility.FromJson<APIResponse<List<EmergencyDtoClass>>>(response.downloadHandler.text);

            foreach (var emer in apiResponse.data)
            {
                SetBooleanValue(emer.type, emer.solved);
            }
        }
        else
        {
            Debug.Log("MainScene CheckEmergency Error"); // Debug Code

            if (response.responseCode == 403)
            {
                RefreshTokenManager.Instance.ReIssueRefreshToken();
                StartCoroutine(CheckEmergency());
            }
            else if (response.responseCode != 404)
            {
                Debug.Log("이미 메인화면이기 때문에 메인화면으로 돌아가는 방식 사용 불가!"); // NEED HELP
                SceneManager.LoadScene("MainScene");
            }
        }
        AfterJudgeEmergency();
    }

    void SetTitleDayCount()
    {
        string createTime = PlayerPrefs.GetString("createTime");
        TimeSpan dateDiff = Convert.ToDateTime(DateTime.Now.ToString("yyyy.MM.dd")) - Convert.ToDateTime(DateTime.Parse(createTime).ToString("yyyy.MM.dd"));
        MainTitleText.text = $"{PlayerPrefs.GetString("dogName")}{GetVerb(PlayerPrefs.GetString("dogName"))} 함께한 지 {dateDiff.Days + 1}일 째 ";

        if (dateDiff.Days + 1 > 56)
        {
            actionable = false;
        }
    }

    private void CloseEndAlertModal()
    {
        EndAlertModal.SetActive(false);
    }

    public void LoadBathScene()
    {
        if (!actionable)
        {
            EndAlertModal.SetActive(true);
            Invoke("CloseEndAlertModal", 2.0f);

            return;
        }

        StartCoroutine(UpdatePosition("BathScene"));
    }

    public void LoadMealScene()
    {
        if (!actionable)
        {
            EndAlertModal.SetActive(true);
            Invoke("CloseEndAlertModal", 2.0f);

            return;
        }

        StartCoroutine(UpdatePosition("MealScene"));
    }

    public void LoadPoopScene()
    {
        if (!actionable)
        {
            EndAlertModal.SetActive(true);
            Invoke("CloseEndAlertModal", 2.0f);

            return;
        }

        StartCoroutine(UpdatePosition("PoopScene"));
    }

    public void LoadDogWalking2DScene()
    {
        if (!actionable)
        {
            EndAlertModal.SetActive(true);
            Invoke("CloseEndAlertModal", 2.0f);

            return;
        }

        StartCoroutine(UpdatePosition("WalkCheckListScene"));
    }

    public void LoadMoveHospitalScene()
    {
        if (!actionable)
        {
            EndAlertModal.SetActive(true);
            Invoke("CloseEndAlertModal", 2.0f);

            return;
        }

        ReceiptScene.type = ReceiptType.HospitalCost1;
        StartCoroutine(UpdatePosition("MoveHospitalScene"));
    }

    public void LoadMoveGroomingScene()
    {
        if (!actionable)
        {
            EndAlertModal.SetActive(true);
            Invoke("CloseEndAlertModal", 2.0f);

            return;
        }

        ReceiptScene.type = ReceiptType.GroomingCost;
        StartCoroutine(UpdatePosition("MoveGroomingScene"));
    }

    public void LoadHeartScene()
    {
        if (!actionable)
        {
            EndAlertModal.SetActive(true);
            Invoke("CloseEndAlertModal", 2.0f);

            return;
        }

        StartCoroutine(UpdatePosition("HeartScene"));
    }

    public void LoadOwnerDictScene()
    {
        SceneManager.LoadScene("OwnerDictScene");
    }

    public void LoadProfileScene()
    {
        SceneManager.LoadScene("ProfileScene");
    }

    public void LoadSettingScene()
    {
        SceneManager.LoadScene("SettingScene");
    }

    void CloseLocationAlertModal()
    {
        LocationAlertModalPanel.SetActive(false);
        EmerPoopModalPanel.SetActive(false);
        EmerBiteModalPanel.SetActive(false);
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            Debug.Log("MainScene - 종료!!!!!\n입장 시간:" + PlayerPrefs.GetString("enterTime"));

            PlayerPrefs.SetString("exitTime", DateTime.Now.ToString("yyyy.MM.dd.HH:mm"));

            Debug.Log("퇴장 시간:" + PlayerPrefs.GetString("exitTime"));

            DateTime enterTime = DateTime.Parse(PlayerPrefs.GetString("enterTime"));
            DateTime exitTime = DateTime.Parse(PlayerPrefs.GetString("exitTime"));
            TimeSpan timeDiff = exitTime - enterTime;

            int diffMinute = timeDiff.Days * 24 * 60 + timeDiff.Hours * 60 + timeDiff.Minutes;

            Debug.Log("OnApplicationPause - diffMinute: " + diffMinute);
            RefreshTokenManager.Instance.UpdateTotalTime(diffMinute);
        }
        else
        {
            Debug.Log("RefreshTokenManager - OnApplicationPause");
            PlayerPrefs.SetString("enterTime", DateTime.Now.ToString("yyyy.MM.dd.HH:mm"));
            Debug.Log("새로운 입장 시간:" + PlayerPrefs.GetString("enterTime"));
        }
    }

    // 현재 위치를 불러오는 메서드
    IEnumerator UpdatePosition(String sceneName)
    {
        LocationManagerObject.SetActive(true);
        while (true)
        {
            // 작업이 완료 되었을 때
            if (locationManager.getIsLocationManagerFinished())
            {
                if (locationManager.getIsAtHome())
                {
                    Debug.Log("집(안) 입니다.");
                    SceneManager.LoadScene(sceneName);
                }
                else
                {
                    Debug.Log("집(밖) 입니다.");

                    LocationAlertModalPanel.SetActive(true);
                    Invoke("CloseLocationAlertModal", 2.0f);
                }
                LocationManagerObject.SetActive(false);
                yield break;
            }
            yield return new WaitForSeconds(1);
        }
    }
}
