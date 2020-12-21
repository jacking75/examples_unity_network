using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIScript : MonoBehaviour
{
    public InputField input1;
    public InputField input2;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void OnClickButton()
    {
        var msgpack = new SimpleMsgPack.MsgPack();
        msgpack.ForcePathObject("input1").AsString = input1.text;
        msgpack.ForcePathObject("input2").AsString = input2.text;
        // pack msgPack binary
        byte[] packData = msgpack.Encode2Bytes();
        
        // unpack msgPack
        var unpack_msgpack = new SimpleMsgPack.MsgPack();
        unpack_msgpack.DecodeFromBytes(packData);

        var print = GameObject.Find("PrintText");
        print.GetComponent<Text>().text = $"{unpack_msgpack.ForcePathObject("input1").AsString} + {unpack_msgpack.ForcePathObject("input2").AsString}";
        //Debug.Log("버튼 클릭");
    }
}
