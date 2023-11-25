function uncloakPassword(ele) {
    ele.classList.add('open');
    ele.previousElementSibling.type = 'text';
}

function cloakPassword(ele) {
    ele.classList.remove('open');
    ele.previousElementSibling.type = 'password';
}


function checkPwRules(ele) {
    const pw = ele.value;
    const validationText = ele.parentElement.nextElementSibling;
    let valid = true;

    if(pw.length < 6) {
        validationText.innerHTML = "Your password must be at least 6 characters long";
        valid = false;
    }else if(!pw.match(/[A-Z]/g)) {
        validationText.innerHTML = "Your password must include uppercase letters";
        valid = false;
    }else if(!pw.match(/[a-z]/)) {
        validationText.innerHTML = "Your password must include lowercase letters";
        valid = false;
    }else if(!pw.match(/\d/)) {
        validationText.innerHTML = "Your password must include numbers";
        valid = false;
    }else if(!pw.match(/[^a-zA-Z0-9]/)) {
        validationText.innerHTML = "Your password must include special characters";
        valid = false;
    }

    if(valid) {
        validationText.parentElement.classList.remove('invalid');
    }else {
        validationText.parentElement.classList.add('invalid');
    }
    return valid;
}

function checkNameRules(ele) {
    const name = ele.value;
    const validationText = ele.parentElement.nextElementSibling;
    let valid = true;
    if(name.length <= 0) {
        validationText.innerHTML = "Name cannot be empty";
        valid = false;
    }else if(name.length > 36) {
        validationText.innerHTML = "Name cannot be longer than 36 characters";
        valid = false;
    }else if(!name.match(/^[a-zA-Z][a-zA-Z0-9\s\-]*$/)) {
        validationText.innerHTML = "Names can only include alphanumeric characters, spaces or dashes!";
        valid = false;
    }
    
    if(valid) {
        validationText.parentElement.classList.remove("invalid");
    }else {
        validationText.parentElement.classList.add("invalid");
    }
    return valid;
}

function checkPwRepeat(eleRepeat, eleCompare) {
    if(eleRepeat.value !== eleCompare.value) {
        eleRepeat.parentElement.parentElement.classList.add('invalid');
        return false;
    }else {
        eleRepeat.parentElement.parentElement.classList.remove('invalid');
        return true;
    }
}

function checkEmail(ele) {
    const pattern = /^([^\x00-\x20\x22\x28\x29\x2c\x2e\x3a-\x3c\x3e\x40\x5b-\x5d\x7f-\xff]+|\x22([^\x0d\x22\x5c\x80-\xff]|\x5c[\x00-\x7f])*\x22)(\x2e([^\x00-\x20\x22\x28\x29\x2c\x2e\x3a-\x3c\x3e\x40\x5b-\x5d\x7f-\xff]+|\x22([^\x0d\x22\x5c\x80-\xff]|\x5c[\x00-\x7f])*\x22))*\x40([^\x00-\x20\x22\x28\x29\x2c\x2e\x3a-\x3c\x3e\x40\x5b-\x5d\x7f-\xff]+|\x5b([^\x0d\x5b-\x5d\x80-\xff]|\x5c[\x00-\x7f])*\x5d)(\x2e([^\x00-\x20\x22\x28\x29\x2c\x2e\x3a-\x3c\x3e\x40\x5b-\x5d\x7f-\xff]+|\x5b([^\x0d\x5b-\x5d\x80-\xff]|\x5c[\x00-\x7f])*\x5d))*$/g
    if(!ele.value.match(pattern)) {
        ele.parentElement.parentElement.classList.add('invalid');
        return false;
    }else {
        ele.parentElement.parentElement.classList.remove('invalid');
        return true;
    }
}

function validateForm(form) {
    let valid = true;
    form.querySelectorAll('.validated-input-container input[type=email]').forEach(ele => {
        valid &&= checkEmail(ele);
    });
    form.querySelectorAll('.validated-input-container .name-input').forEach(input => {
        valid &&= checkNameRules(input);
    });
    form.querySelectorAll('.validated-input-container input[data-input=password]').forEach(ele => {
        let rules = checkPwRules(ele);
        console.log("Passwordcheck",valid, rules);
        valid &&= rules;
        console.log("Passwordcheck",valid);
    });
    form.querySelectorAll('.validated-input-container input[data-input=password-repeat]').forEach(eleRepeat => {
        const inputCompare = document.getElementById(eleRepeat.getAttribute('data-for'));
        valid &&= checkPwRepeat(eleRepeat, inputCompare);
    });
    return valid;
}


document.addEventListener('DOMContentLoaded', () => {
    
    document.querySelectorAll('.error-card').forEach(card => {
        setTimeout(() =>card.classList.add('invisible'), 5000);
    })

    document.querySelectorAll('.password-toggle').forEach(btn => {
        btn.addEventListener('mousedown', () => uncloakPassword(btn));
        btn.addEventListener('mouseup', () => cloakPassword(btn));
        btn.addEventListener('mouseleave', () => cloakPassword(btn));
    });

    document.querySelectorAll('.button-check-box').forEach(btn => {
        const checkbox = document.getElementById(btn.getAttribute('data-for'));
        btn.addEventListener('click', () => {
            if(checkbox.value == 'true') {
                checkbox.value = false;
                btn.classList.remove('active');
            }else {
                checkbox.value = true;
                btn.classList.add('active');
            }
        });
    })

    document.querySelectorAll('.validated-form').forEach(form => {
        form.onsubmit = () => validateForm(form);

        form.querySelectorAll('.validated-input-container input[type=email]').forEach(input => {
            input.addEventListener('blur', () => checkEmail(input));
        });
        
        form.querySelectorAll('.validated-input-container .name-input').forEach(input => {
            input.addEventListener('blur', () => checkNameRules(input));
        });

        form.querySelectorAll('.validated-input-container input[data-input=password]').forEach(input => {
            input.addEventListener('blur', () => checkPwRules(input));
        });

        form.querySelectorAll('.validated-input-container input[data-input=password-repeat]').forEach(input => {
            const inputCompare = document.getElementById(input.getAttribute('data-for'));
            input.addEventListener('blur', () => checkPwRepeat(input, inputCompare));
        });

    })
});