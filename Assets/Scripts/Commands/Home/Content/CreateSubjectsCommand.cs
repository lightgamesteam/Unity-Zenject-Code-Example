using TDL.Models;
using TDL.Views;
using Zenject;

namespace TDL.Commands
{
    public class CreateSubjectsCommand : ICommand
    {
        [Inject] private ContentModel _contentModel;
        [Inject] private HomeModel _homeModel;
        [Inject] private readonly SubjectItemView.Pool _subjectPool;
        [Inject] private readonly SignalBus _signal;

        public void Execute()
        {
            CreateSubjects();
        }

        private void CreateSubjects()
        {
            var sub = _contentModel.GetSubjects(_contentModel.SelectedGrade);

            foreach (var subjectModel in sub)
            {
                var subjectView = _subjectPool.Spawn(_homeModel.TopicsSubtopicsContent);
                subjectView.transform.SetParent(_homeModel.TopicsSubtopicsContent, false);
                subjectView.transform.SetAsLastSibling();

                subjectView.ParentId = _contentModel.SelectedGrade.Grade.id;
                subjectView.Id = subjectModel.Subject.id;
                subjectView.Title.text = subjectModel.Subject.name;

                SaveSubjectsOnHomeScreen(subjectView);
            }

            _signal.Fire(new CreateThumbnailsForSubjectsCommandSignal());
        }

        private void SaveSubjectsOnHomeScreen(SubjectItemView subjectView)
        {
            _homeModel.ShownSubjectOnHome.Add(subjectView);
        }
    }
}