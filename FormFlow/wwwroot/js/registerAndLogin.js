const signUpButton = document.getElementById('signUp')
const signInButton = document.getElementById('signIn')

const signUpForm = document.getElementById('registrationForm')
const signInForm = document.getElementById('loginForm')

const container = document.getElementById('container')

signUpButton.addEventListener('click', () => {
	container.classList.add("right-panel-active")
});

signInButton.addEventListener('click', () => {
	container.classList.remove("right-panel-active")
});

signUpForm.addEventListener('submit', (event) => {
	event.preventDefault();
	signUpForm.setAttribute('action', '/User/Register')
	signUpForm.submit();
})

signInForm.addEventListener('submit', (event) => {
	event.preventDefault();
	signInForm.setAttribute('action', '/User/Login')
	signInForm.submit();
})