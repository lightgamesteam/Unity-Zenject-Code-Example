using TDL.Models;
using TDL.Views;
using Zenject;

namespace TDL.Commands
{
    public class CreateGradesCommand : ICommand
    {
        [Inject] private ContentModel _contentModel;
        [Inject] private HomeModel _homeModel;
        [Inject] private readonly GradeItemView.Pool _gradePool;
        [Inject] private readonly SignalBus _signal;


        public void Execute()
        {
            CreateGrades();
        }

        private void CreateGrades()
        {
            var grd = _contentModel.GetGrades();

            foreach (var clientGradeModel in grd)
            {
                var gradeView = _gradePool.Spawn(_homeModel.TopicsSubtopicsContent);
                gradeView.transform.SetParent(_homeModel.TopicsSubtopicsContent, false);
                gradeView.transform.SetAsLastSibling();

                gradeView.Id = clientGradeModel.Grade.id;
                gradeView.Title.text = clientGradeModel.Grade.name;

                SaveGradesOnHomeScreen(gradeView);
            }

            _signal.Fire(new CreateThumbnailsForGradesCommandSignal());
        }

        private void SaveGradesOnHomeScreen(GradeItemView gradeView)
        {
            _homeModel.ShownGradeOnHome.Add(gradeView);
        }
    }
}