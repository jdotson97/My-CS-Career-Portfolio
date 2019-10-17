// Set the application ID
//const applicationId = sqAppID;
const applicationId = sqAppIDTest;

// onGetCardNonce is triggered when the "Pay $1.00" button is clicked
function onGetCardNonce(event) {
    // Don't submit the form until SqPaymentForm returns with a nonce
    event.preventDefault();
    // Request a nonce from the SqPaymentForm object
    paymentForm.requestCardNonce();
}

// Create and initialize a payment form object
const paymentForm = new SqPaymentForm({
    // Initialize the payment form elements
    applicationId: applicationId,
    inputClass: 'signup-input-text',

    // Customize the CSS for SqPaymentForm iframe elements
    inputStyles: [{
        fontSize: '16px',
        lineHeight: '24px',
        padding: '12px',
        placeholderColor: '#a0a0a0',
        backgroundColor: 'transparent'
    }],

    // Initialize the credit card placeholders
    cardNumber: {
        elementId: 'sq-card-number',
        placeholder: 'Card Number'
    },
    cvv: {
        elementId: 'sq-cvv',
        placeholder: 'CVV'
    },
    expirationDate: {
        elementId: 'sq-expiration-date',
        placeholder: 'MM/YY'
    },
    postalCode: {
        elementId: 'sq-postal-code',
        placeholder: 'Postal'
    },

    // SqPaymentForm callback functions
    callbacks: {
        /*
        * callback function: cardNonceResponseReceived
        * Triggered when: SqPaymentForm completes a card nonce request
        */
        cardNonceResponseReceived: function (errors, nonce, cardData) {
            if (errors) {
                // Log errors from nonce generation to the browser developer console.
                console.error('Encountered errors:');
                errors.forEach(function (error) {
                    console.error('  ' + error.message);
                });
                alert('Encountered errors, check browser developer console for more details');
                return;
            }

            //document.getElementById('zip').value = document.getElementById('sq-postal-code').value;
            document.getElementById('card-nonce').value = nonce;
            document.getElementById('billing-form').submit();          
        }
    }
});
