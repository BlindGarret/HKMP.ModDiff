using System.Collections.Generic;
using Modding;
using Modding.Menu;
using Modding.Menu.Config;
using UnityEngine;
using UnityEngine.UI;
using MenuUtils = Hkmp.ModDiff.Utils.MenuUtils;

namespace Hkmp.ModDiff
{
    internal class MenuMod : Mod, ICustomMenuMod, IGlobalSettings<Configuration>
    {
        private readonly List<IMenuMod.MenuEntry> _menuEntries;
        
        //todo: find a way to dep inject this, not a fan of the static version
        internal static Configuration Configuration = new Configuration();

        public MenuMod() : base(ModInfo.Name)
        {
            _menuEntries = new List<IMenuMod.MenuEntry>
            {
                MenuUtils.BuildEntry(
                    "Match Mode",
                    "Sets match mode for the mod comparison.",
                    type => { Configuration.MatchType = type; },
                    () => Configuration.MatchType
                ),
                MenuUtils.BuildEntry(
                    "Kick on Mismatch",
                    "Tells the server to kick players who get try to join with non-matching mods",
                    type => { Configuration.KickOnMistmatch = type; },
                    () => Configuration.KickOnMistmatch
                )
            };
        }

        public MenuScreen GetMenuScreen(MenuScreen modListMenu, ModToggleDelegates? toggleDelegates)
        {
            var builder = new MenuBuilder(ModInfo.Name);

            builder.CreateTitle(ModInfo.Name, MenuTitleStyle.vanillaStyle);
            builder.CreateContentPane(RectTransformData.FromSizeAndPos(
                new RelVector2(new Vector2(1920f, 903f)),
                new AnchoredPosition(
                    new Vector2(0.5f, 0.5f),
                    new Vector2(0.5f, 0.5f),
                    new Vector2(0f, -60f)
                )
            ));
            builder.CreateControlPane(RectTransformData.FromSizeAndPos(
                new RelVector2(new Vector2(1920f, 259f)),
                new AnchoredPosition(
                    new Vector2(0.5f, 0.5f),
                    new Vector2(0.5f, 0.5f),
                    new Vector2(0f, -502f)
                )
            ));
            builder.SetDefaultNavGraph(new ChainedNavGraph());

            MenuButton backButton = null;
            builder.AddControls(
                new SingleContentLayout(new AnchoredPosition(
                    new Vector2(0.5f, 0.5f),
                    new Vector2(0.5f, 0.5f),
                    new Vector2(0f, -64f)
                )),
                c => c.AddMenuButton(
                    "BackButton",
                    new MenuButtonConfig
                    {
                        Label = Language.Language.Get("OPT_MENU_BACK_BUTTON", "UI"),
                        CancelAction = _ => UIManager.instance.UIGoToDynamicMenu(modListMenu),
                        SubmitAction = _ => UIManager.instance.UIGoToDynamicMenu(modListMenu),
                        Style = MenuButtonStyle.VanillaStyle,
                        Proceed = true
                    },
                    out backButton
                ));

            if (_menuEntries.Count > 5)
            {
                builder.AddContent(new NullContentLayout(), c => c.AddScrollPaneContent(
                    new ScrollbarConfig
                    {
                        CancelAction = _ => UIManager.instance.UIGoToDynamicMenu(modListMenu),
                        Navigation = new Navigation
                        {
                            mode = Navigation.Mode.Explicit,
                            selectOnUp = backButton,
                            selectOnDown = backButton
                        },
                        Position = new AnchoredPosition
                        {
                            ChildAnchor = new Vector2(0f, 1f),
                            ParentAnchor = new Vector2(1f, 1f),
                            Offset = new Vector2(-310f, 0f)
                        }
                    },
                    new RelLength(_menuEntries.Count * 105f),
                    RegularGridLayout.CreateVerticalLayout(105f),
                    cArea => AddMenuEntriesToContentArea(cArea, _menuEntries, modListMenu)
                ));
            }
            else
            {
                builder.AddContent(
                    RegularGridLayout.CreateVerticalLayout(105f),
                    cArea => AddMenuEntriesToContentArea(cArea, _menuEntries, modListMenu)
                );
            }

            return builder.Build();
        }
        
        public static void AddMenuEntriesToContentArea(ContentArea c, IReadOnlyList<IMenuMod.MenuEntry> entries, MenuScreen returnScreen)
        {
            foreach (IMenuMod.MenuEntry entry in entries)
            {
                HorizontalOptionConfig config = new HorizontalOptionConfig
                {
                    ApplySetting = (_, i) => entry.Saver(i),
                    RefreshSetting = (s, _) => s.optionList.SetOptionTo(entry.Loader()),
                    CancelAction = _ => UIManager.instance.UIGoToDynamicMenu(returnScreen),
                    Description = new DescriptionInfo
                    {
                        Text = entry.Description
                    },
                    Label = entry.Name,
                    Options = entry.Values,
                    Style = HorizontalOptionStyle.VanillaStyle
                };

                c.AddHorizontalOption(entry.Name, config, out MenuOptionHorizontal option);
                option.menuSetting.RefreshValueFromGameSettings();
            }
        }

        public override string GetVersion()
        {
            return ModInfo.Version;
        }

        public bool ToggleButtonInsideMenu => false;
        public void OnLoadGlobal(Configuration s)
        {
            //Todo: Add automapper
            Configuration.MatchType = s.MatchType;
            Configuration.KickOnMistmatch = s.KickOnMistmatch;
        }

        public Configuration OnSaveGlobal()
        {
            return Configuration;
        }
    }
}
