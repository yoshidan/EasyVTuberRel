using System;
using System.IO;
using App.Main.Scripts.Interprocess;
using App.Main.Scripts.Interprocess.Model;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using UnityNativeWindow;

namespace App.Scripts.Bg
{
   public class BackgroundImageController : MonoBehaviour
   {

       [Inject] private ReceivedMessageHandler _receivedMessageHandler;

        public GameObject canvasObject;

        private float aspectRatio = 0;
        
        private RawImage rawImage;
        
        private RectTransform imageSize;

        // 横画像を全て表示するかどうか
        private bool widthAll = true;

        private bool shouldSetTransparent = false;
        
        private void Start()
        {

            _receivedMessageHandler.Commands.Subscribe(message =>
            {
                switch (message.Command)
                {
                    case MessageCommandNames.ChangeBackground:
                    {
                        if (message.ToBoolean())
                        {
                            if (canvasObject != null)
                            {
                                canvasObject.SetActive(false);
                            }

                            UnityEngine.Camera.main.backgroundColor = Color.clear;
                            shouldSetTransparent = true;
                        }
                        else
                        {
                            WindowApi.Opacify();
                            shouldSetTransparent = false;
                            if (canvasObject != null)
                            {
                                canvasObject.SetActive(true);
                            }

                            UnityEngine.Camera.main.backgroundColor = Color.green;
                        }

                        break;
                    }
                    case MessageCommandNames.ChangeBackgroundImageDirection:
                    {
                        widthAll = message.ToBoolean();
                        break;
                    }
                }
            });
            
            try
            {
                var path = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/background.jpg";
                if (File.Exists(path))
                {
                    var data = File.ReadAllBytes(path);

                    canvasObject = new GameObject("Background");
                    var imageObject = new GameObject("Image");
                    imageObject.transform.SetParent(canvasObject.transform);
                   
                    var canvas = canvasObject.AddComponent<Canvas>();
                    
                    canvas.renderMode = RenderMode.ScreenSpaceCamera;
                    canvas.worldCamera = UnityEngine.Camera.main;
                    canvasObject.AddComponent<CanvasScaler>();
                    rawImage = imageObject.AddComponent<RawImage>();
                    
                    //画像読み込み
                    var tex = new Texture2D(1,1 );
                    tex.LoadImage(data);
                    
                    //ImageObjectのサイズを画像サイズに合わせる(Canvasをはみ出た分はトリム＝中央クロップになる）
                    imageSize = rawImage.GetComponent<RectTransform>();
                    aspectRatio = (float)tex.height / tex.width;
                    if (widthAll)
                    {
                        imageSize.sizeDelta = new Vector2(Screen.width, Screen.width * aspectRatio);
                    }
                    else
                    {
                        imageSize.sizeDelta = new Vector2(Screen.height / aspectRatio, Screen.height);
                    }

                    rawImage.texture = tex;
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e); 
            }
        }

        private void Update()
        {
            if (canvasObject != null)
            {
                var size = widthAll ? new Vector2(Screen.width, Screen.width * aspectRatio) : new Vector2(Screen.height / aspectRatio, Screen.height);
                if (imageSize.sizeDelta.x.CompareTo(size.x) != 0 || imageSize.sizeDelta.y.CompareTo(size.y) != 0)
                {
                    imageSize.sizeDelta = size;
                }
            }

            // リサイズすると再設定が必要なので
            // nativeのdidResizedNotificationでもダメだった
            if (shouldSetTransparent)
            {
               WindowApi.Transparentize(); 
            }
        }
  
    }
}