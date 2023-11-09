using TDL.Models;
using TDL.Signals;
using Zenject;

namespace TDL.Commands
{
    public class SelectSubjectCommand : ICommandWithParameters
    {
        [Inject] private ContentModel _contentModel;

        public void Execute(ISignal signal)
        {
            var parameter = (SubjectMenuItemClickCommandSignal) signal;

            var selectedSubject = _contentModel.GetSubjectById(parameter.Id);

            if (IsSelectedSubjectDifferentThanAlreadyChosen(selectedSubject)
                || IsSubjectsInDifferentGrades(parameter.ParentId)
                || parameter.IsFromBreadcrumbs)
            {
                ChangeSubject(selectedSubject);
            }
        }

        private bool IsSelectedSubjectDifferentThanAlreadyChosen(ClientSubjectModel selectedSubject)
        {
            return selectedSubject?.Subject.id != _contentModel?.SelectedSubject?.Subject.id;
        }
        
        private bool IsSubjectsInDifferentGrades(int selectedParentId)
        {
            return !selectedParentId.Equals(_contentModel?.PreviousGrade?.Grade.id);
        }

        private void ChangeSubject(ClientSubjectModel selectedSubject)
        {
            _contentModel.SelectedSubject = selectedSubject;
        }
    }
}