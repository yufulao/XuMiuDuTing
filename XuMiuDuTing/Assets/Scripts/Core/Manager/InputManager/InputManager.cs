using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Yu
{
    public class InputManager : BaseSingleTon<InputManager>, IMonoManager
    {
        private readonly InputActions _inputActions = new InputActions();

        public void OnInit()
        {
            _inputActions.Enable();
            //UI是映射列表, Click是一个InputAction名字
            _inputActions.UI.Click.performed += OnClick;
        }

        public void Update()
        {
        }

        public void FixedUpdate()
        {
        }

        public void LateUpdate()
        {
        }

        public void OnClear()
        {
        }

        private void OnClick(InputAction.CallbackContext callbackContext)
        {
            EventManager.Instance.Dispatch(EventName.Click);
        }

        //键盘事件监听
        // void Update()
        // {
        //     if (Keyboard.current.spaceKey.wasPressedThisFrame)
        //     {
        //         Debug.Log("空格键按下");
        //     }
        //     if(Keyboard.current.aKey.wasReleasedThisFrame)
        //     {
        //         Debug.Log("A键抬起");
        //     }
        //     if(Keyboard.current.spaceKey.isPressed)
        //     {
        //         Debug.Log("空格按下");
        //     }
        //     if(Keyboard.current.anyKey.wasPressedThisFrame)
        //     {
        //         Debug.Log("任意键按下");
        //     }
        // }


        //鼠标
        // void Update {
        //     if(Mouse.current.rightButton.wasPressedThisFrame)
        // {
        //     Debug.Log("鼠标右键按下");
        // }
        //
        // if(Mouse.current.middleButton.wasPressedThisFrame)
        // {
        //     Debug.Log("鼠标中建按下");
        // }
        //
        // if(Mouse.current.forwardButton.wasPressedThisFrame)
        // {
        //     Debug.Log("鼠标前键按下");
        // }
        //
        // if(Mouse.current.backButton.wasPressedThisFrame)
        // {
        //     Debug.Log("鼠标后键按下");
        // }
        //
        // //获取鼠标屏幕坐标(左下角为（0,0)
        // Debug.Log(Mouse.current.position.ReadValue());
        //
        // //两帧之间的偏移
        // Debug.Log(Mouse.current.delta.ReadValue());
        //
        // //获取鼠标滚轮坐标
        // Debug.Log(Mouse.current.scroll.ReadValue());


        //触摸屏
        // void Update { Touchscreen ts = Touchscreen. current;
        //     if (ts == null)
        // {
        //     return;
        // }
        //
        // else
        // {
        //     TouchControl tc = ts.touches[0];
        //     if (tc.press.wasPressedThisFrame)
        //     {
        //         Debug.Log("按下");
        //     }
        //
        //     if (tc.press.wasReleasedThisFrame)
        //     {
        //         Debug.Log("抬起");
        //     }
        //
        //     if (tc.press.isPressed)
        //     {
        //         Debug.Log("按住");
        //     }
        //
        //     if (tc.tap.isPressed)
        //     {
        //     }
        //
        //     //点击次数 
        //     Debug.Log(tc.tapCount);
        //     //点击位置
        //     Debug.Log(tc.position.ReadValue());
        //     //第一次接触位置
        //     Debug.Log(tc.startPosition.ReadValue());
        //     //得到的范围
        //     Debug.Log(tc.radius.ReadValue());
        //     //偏移位置
        //     Debug.Log(tc.delta.ReadValue());
        //     //返回TouchPhase: None,Began,Moved,Ended,Canceled,Stationary
        //     Debug.Log(tc.phase.ReadValue());
        //
        //     //判断状态
        //     UnityEngine.InputSystem.TouchPhase tp = tc.phase.ReadValue();
        //     switch (tp)
        //     {
        //         //无
        //         case UnityEngine.InputSystem.TouchPhase.None:
        //             break;
        //         //开始接触
        //         case UnityEngine.InputSystem.TouchPhase.Began:
        //             break;
        //         //移动
        //         case UnityEngine.InputSystem.TouchPhase.Moved:
        //             break;
        //         //结束
        //         case UnityEngine.InputSystem.TouchPhase.Ended:
        //             break;
        //         //取消
        //         case UnityEngine.InputSystem.TouchPhase.Canceled:
        //             break;
        //         //静止
        //         case UnityEngine.InputSystem.TouchPhase.Stationary:
        //             break;
        //     }
        // }


        //手柄
        // Gamepad handle = Gamepad.current;
        //
        //     if(handle==null)
        // {
        //     return;
        // }
        //
        // Vector2 leftDir= handle.leftStick.ReadValue();//左手柄坐标
        // Vector2 rightDir= handle.rightStick.ReadValue();//右手柄坐标
        //
        //     //左摇杆按下抬起
        //     if(Gamepad.current.leftStickButton.wasPressedThisFrame)
        // {
        // }
        // if (Gamepad.current.leftStickButton.wasReleasedThisFrame)
        // {
        // }
        // if (Gamepad.current.leftStickButton.isPressed)
        // {
        // }
        // //右摇杆按下抬起
        // if (Gamepad.current.rightStickButton.wasPressedThisFrame)
        // {
        // }
        // if (Gamepad.current.rightStickButton.wasReleasedThisFrame)
        // {
        // }
        // if (Gamepad.current.rightStickButton.isPressed)
        // {
        // }
        //
        // if(Gamepad.current.dpad.left.wasPressedThisFrame)
        // {
        // }
        // if (Gamepad.current.dpad.left.wasReleasedThisFrame)
        // {
        // }
        // if (Gamepad.current.dpad.left.isPressed)
        // {
        // }
        //
        // //右侧三角方块/XYAB按键
        // //Gamepad.current.buttonEast;
        // //Gamepad.current.buttonWest;
        // //Gamepad.current.buttonSouth;
        // //Gamepad.current.buttonEast;
        // if (Gamepad.current.buttonNorth.wasPressedThisFrame)
        // {
        // }
        // if (Gamepad.current.buttonNorth.wasReleasedThisFrame)
        // {
        // }
        // if (Gamepad.current.buttonNorth.isPressed)
        // {
        // }
        // //手柄中央键
        // if(Gamepad.current.startButton.wasPressedThisFrame)
        // {
        // }
        // if(Gamepad.current.selectButton.wasPressedThisFrame)
        // {
        // }
        // //肩键
        // if(Gamepad.current.leftShoulder.wasPressedThisFrame)
        // {
        // }
        // if (Gamepad.current.rightShoulder.wasPressedThisFrame)
        // {
        // }
        // if(Gamepad.current.leftTrigger.wasPressedThisFrame)
        // {
        // }
        // if(Gamepad.current.rightTrigger.wasPressedThisFrame)
        // {
        // }
    }
}