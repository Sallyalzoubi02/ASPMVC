document.addEventListener('DOMContentLoaded', function () {
    // Handle employment form submission
    const empForm = document.getElementById('employeeForm');
    if (empForm) {
        empForm.addEventListener('submit', function (e) {
            e.preventDefault();

            // Validate employment form
            const empRequiredFields = [
                'empFullName', 'empNationalId', 'empPhone', 'empEmail',
                'empCity', 'empDistrict', 'empPosition', 'empWhy'
            ];

            let empIsValid = true;
            empRequiredFields.forEach(fieldId => {
                const field = document.getElementById(fieldId);
                if (!field.value.trim()) {
                    empIsValid = false;
                    field.classList.add('is-invalid');
                } else {
                    field.classList.remove('is-invalid');
                }
            });

            // Validate experience field (optional but needs to be number if provided)
            const experienceField = document.getElementById('empExperience');
            if (experienceField.value && isNaN(parseInt(experienceField.value))) {
                empIsValid = false;
                experienceField.classList.add('is-invalid');
            } else {
                experienceField.classList.remove('is-invalid');
            }

            if (!empIsValid) {
                showError('يرجى ملء جميع الحقول المطلوبة بشكل صحيح');
                return;
            }

            // Prepare employment form data
            const empFormData = {
                FullName: document.getElementById('empFullName').value.trim(),
                NationalId: document.getElementById('empNationalId').value.trim(),
                Phone: document.getElementById('empPhone').value.trim(),
                Email: document.getElementById('empEmail').value.trim(),
                City: document.getElementById('empCity').value.trim(),
                District: document.getElementById('empDistrict').value.trim(),
                ExperienceYears: experienceField.value ? parseInt(experienceField.value) : null,
                Position: document.getElementById('empPosition').value.trim(),
                MotivationText: document.getElementById('empWhy').value.trim()
            };

            submitForm('/Join/SubmitEmployment', empFormData, empForm);
        });
    }

    // Handle volunteer form submission
    const volForm = document.getElementById('volunteerForm');
    if (volForm) {
        volForm.addEventListener('submit', function (e) {
            e.preventDefault();

            // Validate volunteer form
            const volRequiredFields = [
                'volFullName', 'volPhone', 'volEmail',
                'volCity', 'volAge', 'volWhy'
            ];

            let volIsValid = true;
            volRequiredFields.forEach(fieldId => {
                const field = document.getElementById(fieldId);
                if (!field.value.trim()) {
                    volIsValid = false;
                    field.classList.add('is-invalid');
                } else {
                    field.classList.remove('is-invalid');
                }
            });

            // Validate age field
            const ageField = document.getElementById('volAge');
            if (isNaN(parseInt(ageField.value)) || parseInt(ageField.value) < 18) {
                volIsValid = false;
                ageField.classList.add('is-invalid');
            } else {
                ageField.classList.remove('is-invalid');
            }

            // Validate agreement checkbox
            const agreement = document.getElementById('volAgreement');
            if (!agreement.checked) {
                volIsValid = false;
                agreement.classList.add('is-invalid');
            } else {
                agreement.classList.remove('is-invalid');
            }

            // Validate availability days
            const availabilitySelect = document.getElementById('volAvailability');
            const selectedDays = Array.from(availabilitySelect.selectedOptions).map(option => option.value);
            if (selectedDays.length === 0) {
                volIsValid = false;
                availabilitySelect.classList.add('is-invalid');
            } else {
                availabilitySelect.classList.remove('is-invalid');
            }

            if (!volIsValid) {
                showError('يرجى ملء جميع الحقول المطلوبة بشكل صحيح');
                return;
            }

            // Prepare volunteer form data
            // Prepare volunteer form data - النسخة المصححة
            const volFormData = {
                FullName: document.getElementById('volFullName').value.trim(),
                NationalId: document.getElementById('volNationalId').value.trim(),
                Phone: document.getElementById('volPhone').value.trim(),
                Email: document.getElementById('volEmail').value.trim(),
                City: document.getElementById('volCity').value.trim(),
                District: document.getElementById('volDistrict').value.trim(),
                Age: parseInt(document.getElementById('volAge').value),
                AvailabilityDays: Array.from(document.getElementById('volAvailability').selectedOptions)
                    .map(option => option.value),
                Skills: document.getElementById('volSkills').value.trim(),
                MotivationText: document.getElementById('volWhy').value.trim(),
                AgreementAccepted: true
            };

            // تأكيد أن البيانات صحيحة قبل الإرسال
            console.log('Data to be sent:', JSON.stringify(volFormData, null, 2));

            submitForm('/Join/SubmitVolunteer', volFormData, volForm);
        });
    }

    // Generic form submission function
    function submitForm(url, data, formElement) {
        showLoading(); // Show loading indicator

        fetch(url, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
            },
            body: JSON.stringify(data)
        })
            .then(handleResponse)
            .then(result => {
                if (result.success) {
                    showSuccess(result.message || 'تم إرسال البيانات بنجاح');
                    formElement.reset();
                } else {
                    showError(result.message || 'حدث خطأ أثناء معالجة الطلب');
                }
            })
            .catch(error => {
                console.error('Error:', error);
                showError('حدث خطأ في الاتصال بالخادم. يرجى المحاولة مرة أخرى.');
            });
    }

    // Response handler
    function handleResponse(response) {
        if (!response.ok) {
            return response.json().then(err => {
                throw new Error(err.message || 'Network response was not ok');
            });
        }
        return response.json();
    }

    // UI Helpers
    function showLoading() {
        Swal.fire({
            title: 'جارٍ المعالجة...',
            allowOutsideClick: false,
            didOpen: () => {
                Swal.showLoading();
            }
        });
    }

    function showSuccess(message) {
        Swal.fire({
            icon: 'success',
            title: 'تم بنجاح!',
            text: message,
            confirmButtonText: 'حسناً'
        });
    }

    function showError(message) {
        Swal.fire({
            icon: 'error',
            title: 'خطأ',
            text: message,
            confirmButtonText: 'حسناً'
        });
    }

    // Add input event listeners to remove invalid class when user starts typing
    document.querySelectorAll('input, select, textarea').forEach(element => {
        element.addEventListener('input', function () {
            this.classList.remove('is-invalid');
        });
    });
});