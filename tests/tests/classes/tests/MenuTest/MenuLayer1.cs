/****************************************************************************
Copyright (c) 2010-2012 cocos2d-x.org
Copyright (c) 2008-2009 Jason Booth
Copyright (c) 2011-2012 openxlive.com

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cocos2d;
using System.Diagnostics;
using cocos2d.menu_nodes;

namespace tests
{
    public class MenuLayer1 : CCLayer
    {
        protected CCMenuItemLabel m_disabledItem;
        string s_SendScore = "Images/SendScoreButton";
        string s_MenuItem = "Images/menuitemsprite";
        string s_PressSendScore = "Images/SendScoreButtonPressed";

        public MenuLayer1()
        {
            CCMenuItemFont.FontSize = 30;
            CCMenuItemFont.FontName = "arial";
            base.TouchEnabled = true;
            // Font Item

            CCSprite spriteNormal = CCSprite.Create(s_MenuItem, new CCRect(0, 23 * 2, 115, 23));
            CCSprite spriteSelected = CCSprite.Create(s_MenuItem, new CCRect(0, 23 * 1, 115, 23));
            CCSprite spriteDisabled = CCSprite.Create(s_MenuItem, new CCRect(0, 23 * 0, 115, 23));

            CCMenuItemSprite item1 = CCMenuItemSprite.Create(spriteNormal, spriteSelected, spriteDisabled, this.menuCallback);

            // Image Item
            CCMenuItem item2 = CCMenuItemImage.Create(s_SendScore, s_PressSendScore, this.menuCallback2);

            // Label Item (LabelAtlas)
            CCLabelAtlas labelAtlas = CCLabelAtlas.Create("0123456789", "Images/fps_Images", 16, 24, '.');
            CCMenuItemLabel item3 = CCMenuItemLabel.Create(labelAtlas, this.menuCallbackDisabled);
            item3.DisabledColor = new CCColor3B(32, 32, 64);
            item3.Color = new CCColor3B(200, 200, 255);

            // Font Item
            CCMenuItemFont item4 = CCMenuItemFont.Create("I toggle enable items", this.menuCallbackEnable);

            item4.FontSizeObj = 20;
            item4.FontNameObj = "arial";

            // Label Item (CCLabelBMFont)
            CCLabelBMFont label = CCLabelBMFont.Create("configuration", "fonts/bitmapFontTest3.fnt");
            CCMenuItemLabel item5 = CCMenuItemLabel.Create(label, this.menuCallbackConfig);
            

            // Testing issue #500
            item5.Scale = 0.8f;

            // Events
            CCMenuItemFont.FontName = "arial";
            CCMenuItemFont item6 = CCMenuItemFont.Create("Priority Test", menuCallbackPriorityTest);

            // Font Item
            CCMenuItemFont item7 = CCMenuItemFont.Create("Quit", this.onQuit);

            CCActionInterval color_action = CCTintBy.Create(0.5f, 0, -255, -255);
            CCActionInterval color_back = (CCActionInterval)color_action.Reverse();
            CCFiniteTimeAction seq = CCSequence.Create(color_action, color_back);
            item7.RunAction(CCRepeatForever.Create((CCActionInterval)seq));

            CCMenu menu = CCMenu.Create(item1, item2, item3, item4, item5, item6, item7);
            menu.AlignItemsVertically();

            // elastic effect
            CCSize s = CCDirector.SharedDirector.WinSize;
            int i = 0;
            CCNode child;
            var pArray = menu.Children;
            object pObject = null;
            if (pArray.Count > 0)
            {
                for (int j = 0; j < pArray.Count; j++)
                {
                    pObject = pArray[j];
                    if (pObject == null)

                        break;
                    child = (CCNode)pObject;
                    CCPoint dstPoint = child.Position;
                    int offset = (int)(s.Width / 2 + 50);
                    if (i % 2 == 0)
                        offset = -offset;

                    child.Position = new CCPoint(dstPoint.X + offset, dstPoint.Y);
                    child.RunAction(CCEaseElasticOut.Create(CCMoveBy.Create(2, new CCPoint(dstPoint.X - offset, 0)), 0.35f));
                    i++;

                }
            }
            m_disabledItem = item3;
            m_disabledItem.Enabled = false;

            AddChild(menu);
        }


        void menuCallbackPriorityTest(object pSender)
        {
            ((CCLayerMultiplex)m_pParent).SwitchTo(4);
        }

        public override void RegisterWithTouchDispatcher()
        {
            CCDirector.SharedDirector.TouchDispatcher.AddTargetedDelegate(this, -128 + 1, true);
        }
        public override bool TouchBegan(CCTouch touch, CCEvent pEvent)
        {
            return true;
        }

        public override void TouchEnded(CCTouch touch, CCEvent pEvent)
        {
        }

        public override void TouchCancelled(CCTouch touch, CCEvent pEvent)
        {
        }

        public override void TouchMoved(CCTouch touch, CCEvent pEvent)
        {
        }

        public void allowTouches(float dt)
        {
            CCDirector.SharedDirector.TouchDispatcher.SetPriority(-128 + 1, this);
            base.UnscheduleAllSelectors();
            CCLog.Log("TOUCHES ALLOWED AGAIN");
        }
        public void menuCallback(object pSender)
        {
            ((CCLayerMultiplex)m_pParent).SwitchTo(1);
        }
        public void menuCallbackConfig(object pSender)
        {
            ((CCLayerMultiplex)m_pParent).SwitchTo(3);
        }
        public void menuCallbackDisabled(object pSender)
        {
            // hijack all touch events for 5 seconds
            CCDirector.SharedDirector.TouchDispatcher.SetPriority(-128 - 1, this);
            base.Schedule(this.allowTouches, 5.0f);
            CCLog.Log("TOUCHES DISABLED FOR 5 SECONDS");
        }
        public void menuCallbackEnable(object pSender)
        {
            m_disabledItem.Enabled = !m_disabledItem.Enabled;
        }
        public void menuCallback2(object pSender)
        {
            (m_pParent as CCLayerMultiplex).SwitchTo(2);
        }
        public void onQuit(object pSender)
        {
            //[[Director sharedDirector] end];
            //getCocosApp()->exit();
        }
    }
}
