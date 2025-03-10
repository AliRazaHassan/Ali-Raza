﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

namespace EasyUI.Tabs
{
    public enum TabsType
    {
        Horizontal,
        Vertical
    }

   
    public abstract class TabsUI : MonoBehaviour
    {
        [System.Serializable]
        public class TabsUIEvent : UnityEvent<int> { }

        [Header("Tabs Customization")]
        [SerializeField]
        private Color m_ThemeColor = Color.gray;

        [SerializeField]
        private float m_TabSpacing = 2f;

        [SerializeField]
        private bool m_IgnoreThemeColor;

        [Space]
        [Header("OnTabChange Events")]
        public TabsUIEvent m_OnTabChange;

        [Header("Containers")]
        [SerializeField]
        private RectTransform m_TabButtonsContainer;

        [SerializeField]
        private RectTransform m_TabContentsContainer;
        [SerializeField]
        private TabButtonUI[] tabButtons;
        [SerializeField]
        private RectTransform[] tabContents;

        private int tabButtonsCount,
            tabContentsCount;


        private LayoutGroup tabButtonsLayoutGroup;


        private Color tabColorActive,
            tabColorInactive;
        private int current,
            previous;

        private void Reset()
        {
            if (!m_TabButtonsContainer)
            {
                m_TabButtonsContainer = (RectTransform)transform.GetChild(0);
                Debug.Log(
                    $"Automatically assign <b>Tab Buttons Container</b> with [<b>{m_TabButtonsContainer.name}</b>]. If it\'s not the right GameObject, please assign it manually."
                );
            }

            if (!m_TabContentsContainer)
            {
                m_TabContentsContainer = (RectTransform)transform.GetChild(1);
                Debug.Log(
                    $"Automatically assign <b>Tab Contents Container</b> with [<b>{m_TabContentsContainer.name}</b>]. If it\'s not the right GameObject, please assign it manually."
                );
            }
        }

        private void Awake()
        {
            /*StartCoroutine( SetupTabButtons());
         }*/
        }
        private void Initialize()
        {

           
            if (!m_TabButtonsContainer)
                Debug.LogError(
                    "<b>Tab Buttons Container</b> is missing! Please assign it with the parent of <b>Tab Buttons</b>."
                );

            if (!m_TabContentsContainer)
                Debug.LogError(
                    "<b>Tab Contents Container</b> is missing! Please assign it with the parent of <b>Tab Contents</b>."
                );

            if (!m_TabButtonsContainer || !m_TabContentsContainer)
            {
                Debug.LogError(
                   "<b>Tab Contents Container</b> is missing! m_TabButtonsContainer <b>Tab Contents</b>.");
                return;
            }

            tabButtonsCount = m_TabButtonsContainer.childCount;
            tabContentsCount = m_TabContentsContainer.childCount;
/*
            if (tabButtonsCount != tabContentsCount)
            {
                Debug.LogError("Tab Buttons and Tab Contents counts do not match. Check your setup."+tabButtonsCount+"TAB"+tabContentsCount );
                return;
            }*/


            tabButtons = m_TabButtonsContainer.GetComponentsInChildren<TabButtonUI>();
            tabContents = new RectTransform[tabContentsCount];


          for (int i = 0; i < tabContentsCount; i++)
            {

                if ((RectTransform)m_TabContentsContainer.GetChild(i) != null)
                {
                    tabContents[i] = (RectTransform)m_TabContentsContainer.GetChild(i);
                }
            }
 
            if (!tabButtonsLayoutGroup)
                tabButtonsLayoutGroup = m_TabButtonsContainer.GetComponent<LayoutGroup>();
    
        }

        private IEnumerator SetupTabButtons()
        {
            yield return new WaitForSeconds(.1f);


          //  Initialize();
            if (tabButtons == null || tabContents == null)
            {
                Debug.LogError("Initialization failed. Check your Tab Buttons and Tab Contents setup.");
                yield break; // Exit the coroutine if initialization fails
            }
                for (int i = 0; i < tabButtonsCount; i++)
            {
                int i_copy = i;
                if (tabButtons[i] != null)
                {
                    Debug.Log(tabButtons[i].gameObject.name);
                    tabButtons[i].button.onClick.RemoveAllListeners();
                    tabButtons[i].button.onClick.AddListener(() => OnTabButtonClicked(i_copy));
                    Debug.Log(tabContents[i].gameObject.name);
                    tabContents[i].gameObject.SetActive(false);
                }
            }

            previous = current = 0;

            tabColorActive = tabButtons[0].image.color;
            tabColorInactive = tabButtons[1].image.color;

            tabButtons[0].button.interactable = false;
            tabContents[0].gameObject.SetActive(true);
        }

        private void OnTabButtonClicked(int tabIndex)
        {
            Debug.Log("this is working ");
            Debug.Log("Tab Index: " + tabIndex);
            if (current != tabIndex)
            {
                m_OnTabChange?.Invoke(tabIndex);

                previous = current;
                current = tabIndex;

                tabContents[current].gameObject.SetActive(true);
                tabContents[previous].gameObject.SetActive(false);

                tabButtons[current].image.color = tabColorActive;
                tabButtons[previous].image.color = tabColorInactive;

                tabButtons[previous].button.interactable = true;
                tabButtons[current].button.interactable = false;
            }
        }


        public void Validate(TabsType tabType)
        {
            Initialize();
           
            UpdateThemeColor(m_ThemeColor);

            // Set tab spacing to LayoutGroup component.
            //switch (tabType)
            //{
            //    case TabsType.Horizontal:
            //        ((HorizontalLayoutGroup)tabButtonsLayoutGroup).spacing = m_TabSpacing;
            //        break;
            //  case TabsType.Vertical:
            //        ((VerticalLayoutGroup)tabButtonsLayoutGroup).spacing = m_TabSpacing;
            //        break;
            //}
        }

        private void UpdateThemeColor(Color color)
        {
            if (m_IgnoreThemeColor) return;

            Color colorDark = DarkenColor(color, 0.3f);

            if (tabButtons[0].image)
                tabButtons[0].image.color = color;

            for (int i = 1; i < tabButtonsCount; i++)
            {
                if (tabButtons[i].image)
                    tabButtons[i].image.color = colorDark;
            }

            m_TabContentsContainer.GetComponent<Image>().color = color;
        }

        private Color DarkenColor(Color color, float amount)
        {
            float h,
                s,
                v;
            Color.RGBToHSV(color, out h, out s, out v);

            v = Mathf.Max(0f, v - amount);

            return Color.HSVToRGB(h, s, v);
        }

    }
}
