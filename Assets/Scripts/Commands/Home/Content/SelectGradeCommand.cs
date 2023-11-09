using System.Linq;
using TDL.Models;
using TDL.Signals;
using Zenject;

namespace TDL.Commands
{
    public class SelectGradeCommand : ICommandWithParameters
    {
        [Inject] private ContentModel _contentModel;

        public void Execute(ISignal signal)
        {
            switch (signal)
            {
                case GradeMenuItemClickCommandSignal grade:
                    SavePreviousSelectedGrade();
                    SetSelectedGrade(grade.Id);
                    break;
            
                case SubjectMenuItemClickCommandSignal subject:
                    SavePreviousSelectedGrade();
                    SetSelectedGrade(subject.ParentId);
                    break;
            }
        }

        private void SavePreviousSelectedGrade()
        {
            _contentModel.PreviousGrade = _contentModel.SelectedGrade;
        }

        private void SetSelectedGrade(int parentId)
        {
            var selectedGrade = _contentModel.GetGrades().FirstOrDefault(item => item.Grade.id == parentId);

            if (IsSelectedGradeDifferentThanAlreadyChosen(selectedGrade))
            {
                _contentModel.SelectedGrade = selectedGrade;
            }
        }
    
        private bool IsSelectedGradeDifferentThanAlreadyChosen(ClientGradeModel selectedGrade)
        {
            return selectedGrade?.Grade.id != _contentModel?.SelectedGrade?.Grade.id;
        }
    }
}