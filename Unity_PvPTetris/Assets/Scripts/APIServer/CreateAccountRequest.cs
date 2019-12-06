using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using UnityEngine.UI;
using LitJson;
using LobbyServer;

namespace APIServer
{

    public class CreateAccountRequest : MonoBehaviour
    {
        JsonData result_json;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public class CreateuserResult
        {
            public short Result;
        }

        class LoginResultData
        {
            public short Result;
            public string AuthToken;
        }


        // 아래는 Redis를 사용하는 API서버를 통해 로그인을 하는 함수
        // TODO 현재 클라이언트에서는 테스트를 위해 이 함수가 아니라 LoginBtnClicket

        public void SendCreateAccountRequest()
        {
            var input_id = (GameObject.Find("input_id_field")).GetComponent<InputField>().text;
            var input_pw = (GameObject.Find("input_pw_field")).GetComponent<InputField>().text;

            if (input_id == "")
            {
                //아이디를 입력해주세요;
                return;
            }

            if (input_pw == "")
            {
                //비밀번호를 입력해주세요
                return;
            }

            string data = "{\"UserID\":\"" + input_id + "\", \"UserPW\":\"" + input_pw + "\"}";

            //TODO 로그인서버도 ip 받아오게 수정하기
            StartCoroutine(Post("localhost:19000/api/Account/Create", data));
        }


        IEnumerator Post(string url, string bodyJsonString)
        {
            var request = new UnityWebRequest(url, "POST");
            byte[] bodyRaw = Encoding.UTF8.GetBytes(bodyJsonString);
            request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            yield return request.SendWebRequest();

            Debug.Log("text: " + request.downloadHandler.text);
            result_json = JsonMapper.ToObject(request.downloadHandler.text);

            short result = short.Parse(result_json["result"].ToString());
            
            if (result > 0) //0을 Success Code로 설정하였음
            {
                Debug.Log("회원가입 성공");
            }
            else
            {
                Debug.Log("회원가입 실패"+result);
            }
        }
    }

}


