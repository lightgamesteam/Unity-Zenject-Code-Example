using TDL.Models;
using TDL.Server;
using TDL.Views;
using UnityEngine;
using Zenject;

namespace TDL.Commands
{
    public class CreateLeftMenuGradesSubjectsCommand : ICommand
    {
        [Inject] private ContentModel _contentModel;
        [Inject] private HomeModel _homeModel;
        [Inject] private readonly GradeMenuItemView.Factory _gradeFactory;
        [Inject] private readonly SubjectMenuItemView.Factory _subjectFactory;
        [Inject] private SubjectMenuItemViewsMediator _subjectMenuItemViewMediator;

        private const string SubjectMenuItem = "SubjectMenuItem | " ;

        public void Execute()
        {
            CreateGradesSubjectsMenu();
        }

        private void CreateGradesSubjectsMenu()
        {
            foreach (var gradeModel in _contentModel.GetGrades())
            {
                var gradeView = CreateGradeContainer(gradeModel.Grade, _homeModel.LeftMenuContent);
                var subjectContainerHeight = CreateSubjectItems(gradeModel, gradeView);
                SetGradeContainerHeight(gradeView, subjectContainerHeight);

                if (DeviceInfo.IsPCInterface())
                {
                    SetTooltip(gradeView, gradeModel.Grade.name);
                }
            }
        }

        private GradeMenuItemView CreateGradeContainer(Grade gradeModel, Transform leftMenuContent)
        {
            var gradeView = _gradeFactory.Create();
            gradeView.Title.text = gradeModel.name;
            gradeView.transform.SetParent(leftMenuContent, false);

            return gradeView;
        }

        private float CreateSubjectItems(ClientGradeModel gradeModel, GradeMenuItemView gradeView)
        {
            float subjectContainerHeight = 0;
        
            foreach (var subjectModel in _contentModel.GetSubjects(gradeModel))
            {
                var subjectView = _subjectFactory.Create();
                subjectView.ParentId = gradeModel.Grade.id;
                subjectView.Id = subjectModel.Subject.id;
                var subjectName = subjectModel.Subject.name;
                subjectView.SetSubjectName(subjectName);
                subjectView.gameObject.name = SubjectMenuItem + subjectName;
                subjectView.transform.SetParent(gradeView.GetSubjectsContent(), false);
                subjectContainerHeight += subjectView.GetComponent<RectTransform>().sizeDelta.y;

                var key = subjectModel.ParentGrade.Grade.id + " " + subjectModel.Subject.id;
                if (!_homeModel.SelectableMenuItems.ContainsKey(key))
                {
                    _homeModel.SelectableMenuItems.Add(subjectModel.ParentGrade.Grade.id + " " + subjectModel.Subject.id,
                        subjectView);
                }
            
                if (DeviceInfo.IsPCInterface())
                {
                    SetTooltip(subjectView, subjectName);
                
                    if (DeviceInfo.IsTablet())
                        SetTabletHighlightedColor(subjectView);
                }
            }
        
            return subjectContainerHeight;
        }
        
        private void SetTabletHighlightedColor(SubjectMenuItemView subjectMenuItem)
        {
            subjectMenuItem.normalColorBlock.highlightedColor = Color.clear;
            subjectMenuItem.selectedColorBlock.highlightedColor = Color.clear;
        
            subjectMenuItem.normalColorBlock.pressedColor = Color.clear;
            subjectMenuItem.selectedColorBlock.pressedColor = Color.clear;
        
            subjectMenuItem.normalColorBlock.fadeDuration = 0f;
            subjectMenuItem.selectedColorBlock.fadeDuration = 0f;
        }

        private void SetGradeContainerHeight(GradeMenuItemView gradeView, float subjectContainerHeight)
        {
            var gradeTransform = gradeView.GetComponent<RectTransform>();
            gradeTransform.sizeDelta = new Vector2(gradeTransform.sizeDelta.x,
                gradeTransform.sizeDelta.y + subjectContainerHeight);
        }

        private void SetTooltip(ViewBase view, string titleName)
        {
            var tooltip = view.gameObject.GetComponentInChildren<DynamicTooltipEvents>();
            tooltip.SetHint(titleName);
        }
    }
}