import { AbstractControl } from "@angular/forms";

export function passwordValidator(control:AbstractControl): { [key :string] : boolean} | null {
  const newPassword = control.get('newPassword');
  const confirmPassword = control.get('confirmPassword');
  if (newPassword.pristine || confirmPassword.pristine)
  {
    return null;
  }
  if(newPassword && confirmPassword && newPassword.value != confirmPassword.value)
  {
    console.log("hey")
  }
  return newPassword && confirmPassword && newPassword.value != confirmPassword.value ?
    {'mismatch' : true} :
    null;
}
