using Graphene.UiGenerics;

namespace Graphene.Rhythm.Presentation
{
    public class RestartGameButton : ButtonView
    {
        private MenuManager _menuManager;

        void Setup()
        {
            _menuManager = FindObjectOfType<MenuManager>();
        }
        
        
        protected override void OnClick()
        {
            base.OnClick();
            
            _menuManager.RestartGame();
        }
    }
}