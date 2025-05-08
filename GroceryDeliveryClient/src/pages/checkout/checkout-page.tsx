import { Separator } from '@/components/ui/separator';
import type { ShoppingBasket } from '@/types';
import { useEffect, useMemo, useState, type FC } from 'react';
import PlaceholderImage from '@/assets/placeholder-image.svg';
import { Button } from '@/components/ui/button';
import { useNavigate } from 'react-router-dom';
import { useShoppingBasket } from '@/hooks/shopping-basket';
import { useCreateOrder } from '@/api/queries/order-queries'; 

const PACKING_FEE = 4.51;
const DELIVERY_FEE = 10.99;
const SALES_TAX_PERCENT = 25;

const quantityTotal = (price: number, quantity: number): string => {
    const total = price * quantity;
    return total.toFixed(2);
};

const calcSubtotal = (shoppingBasket: ShoppingBasket): number => {
    return shoppingBasket.reduce((total, item) => {
        return total + item.product.price * item.quantity;
    }, 0);
};

const calcTotal = (price: number): number => {
    const withSalesTax = price * (1 + SALES_TAX_PERCENT / 100);
    return withSalesTax + PACKING_FEE + DELIVERY_FEE;
};

export const CheckoutPage: FC = () => {
    const navigate = useNavigate();
    const { basket, clearBasket } = useShoppingBasket();
    const createOrder = useCreateOrder();

    // Form state
    const [customerName, setCustomerName] = useState('');
    const [customerEmail, setCustomerEmail] = useState('');
    const [customerPhone, setCustomerPhone] = useState('');
    const [Address, setCustomerAddress] = useState('');
    const [City, setCustomerCity] = useState('');
    const [ZipCode, setCustomerPostalCode] = useState('');
    const [Country, setCustomerCountry] = useState('');
    const [paymentMethod, setPaymentMethod] = useState('Credit Card');


    const subtotal: number = useMemo(() => {
        return calcSubtotal(basket);
    }, [basket]);

    useEffect(() => {
        if (basket.length <= 0 && !createOrder.isSuccess) { // Prevent redirect if order was just placed
            void navigate('/');
        }
    }, [basket, navigate, createOrder.isSuccess]);

    const handlePlaceOrder = async () => {
        if (basket.length === 0) {
            // Optionally, show an error message if the basket is empty
            console.error("Basket is empty. Cannot place order.");
            return;
        }

        const orderData = {
            // Ensure this is the correct field name (userId or customerId) and value
            // for the customer identifier that the backend expects.
            userId: 1, // Or potentially customerId: 1 if the backend expects that
            address: Address, // Changed from Address to address
            city: City,       // Changed from City to city
            zipCode: parseInt(ZipCode, 10) || 0, // Key is zipCode
            country: Country, // Changed from Country to country
            paymentMethod: paymentMethod,
            orderItems: basket.map(item => ({
                productId: item.product.productId, // Ensure productId is sent
                quantity: item.quantity,
            })),
            // Removed: id, customerName, customerEmail, customerPhone, orderStatus, totalAmount
            // as they are not in the API creation schema you provided.
        };
        
        try {
            await createOrder.mutateAsync(orderData); 
            // Navigate to order confirmation page on success
            void navigate('/order-confirmation');
            // Clear the basket after a short delay to allow navigation
            setTimeout(() => {
                clearBasket();
            }, 100);
        } catch (error) {
            console.error("Failed to place order:", error);
        }
    };

    return (
        <div className="container mx-auto p-4">
            <h1 className="text-3xl font-bold mb-6">Checkout</h1>
            <div className="grid grid-cols-1 md:grid-cols-3 gap-8">
                <div className="md:col-span-2">
                    {/* User Information */}
                    <section className="mb-8">
                        <h2 className="text-2xl font-semibold mb-4">User Information</h2>
                        <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
                            <div>
                                <label htmlFor="name" className="block text-sm font-medium text-gray-700">Full Name</label>
                                <input
                                    type="text"
                                    name="name"
                                    id="name"
                                    className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-indigo-500 focus:ring-indigo-500 sm:text-sm py-2"
                                    value={customerName}
                                    onChange={(e) => {setCustomerName(e.target.value)}}
                                    required
                                />
                            </div>
                            <div>
                                <label htmlFor="email" className="block text-sm font-medium text-gray-700">Email Address</label>
                                <input
                                    type="email"
                                    name="email"
                                    id="email"
                                    className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-indigo-500 focus:ring-indigo-500 sm:text-sm py-2"
                                    value={customerEmail}
                                    onChange={(e) => {setCustomerEmail(e.target.value)}}
                                    required
                                />
                            </div>
                            <div>
                                <label htmlFor="phone" className="block text-sm font-medium text-gray-700">Phone Number</label>
                                <input
                                    type="tel"
                                    name="phone"
                                    id="phone"
                                    className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-indigo-500 focus:ring-indigo-500 sm:text-sm py-2"
                                    value={customerPhone}
                                    onChange={(e) => {setCustomerPhone(e.target.value)}}
                                    required
                                />
                            </div>
                        </div>
                    </section>

                    <Separator className="my-6" />

                    {/* Shipping Details */}
                    <section className="mb-8">
                        <h2 className="text-2xl font-semibold mb-4">Shipping Details</h2>
                        <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
                            <div>
                                <label htmlFor="address" className="block text-sm font-medium text-gray-700">Address</label>
                                <input
                                    type="text"
                                    name="address"
                                    id="address"
                                    className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-indigo-500 focus:ring-indigo-500 sm:text-sm py-2"
                                    value={Address}
                                    onChange={(e) => {setCustomerAddress(e.target.value)}}
                                    required
                                />
                            </div>
                            <div>
                                <label htmlFor="city" className="block text-sm font-medium text-gray-700">City</label>
                                <input
                                    type="text"
                                    name="city"
                                    id="city"
                                    className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-indigo-500 focus:ring-indigo-500 sm:text-sm py-2"
                                    value={City}
                                    onChange={(e) => {setCustomerCity(e.target.value)}}
                                    required
                                />
                            </div>
                            <div>
                                <label htmlFor="postalCode" className="block text-sm font-medium text-gray-700">Postal Code</label>
                                <input
                                    type="text"
                                    name="postalCode"
                                    id="postalCode"
                                    className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-indigo-500 focus:ring-indigo-500 sm:text-sm py-2"
                                    value={ZipCode}
                                    onChange={(e) => {setCustomerPostalCode(e.target.value)}}
                                    required
                                />
                            </div>
                            <div>
                                <label htmlFor="country" className="block text-sm font-medium text-gray-700">Country</label>
                                <input
                                    type="text"
                                    name="country"
                                    id="country"
                                    className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-indigo-500 focus:ring-indigo-500 sm:text-sm py-2"
                                    value={Country}
                                    onChange={(e) => {setCustomerCountry(e.target.value)}}
                                    required
                                />
                            </div>
                        </div>
                    </section>

                    <Separator className="my-6" />

                    {/* Payment Options */}
                    <section>
                        <h2 className="text-2xl font-semibold mb-4">Payment Method</h2>
                        <div>
                            <label htmlFor="paymentMethod" className="block text-sm font-medium text-gray-700">Select Payment Option</label>
                            <select
                                id="paymentMethod"
                                name="paymentMethod"
                                className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-indigo-500 focus:ring-indigo-500 sm:text-sm py-2"
                                value={paymentMethod}
                                onChange={(e) => {setPaymentMethod(e.target.value)}}
                            >
                                <option>Credit Card</option>
                                <option>Bank Transfer</option>
                                <option>Cash</option>
                            </select>
                        </div>
                    </section>
                </div>

                {/* Order Summary */}
                <div className="md:col-span-1 bg-gray-50 p-6 rounded-lg shadow flex flex-col">
                    <div className="flex-grow">
                        <h2 className="text-2xl font-semibold mb-4">Order Summary</h2>
                        {basket.map((item) => (
                            <div key={item.product.productId} className="flex justify-between items-center mb-2"> {/* Ensure key is unique, using productId */}
                                <div className="flex items-center">
                                    <img
                                        src={item.product.imagePath ?? PlaceholderImage}
                                        alt={item.product.productName} // Use productName
                                        className="w-12 h-12 object-cover rounded mr-3 "
                                        onError={(e) => (e.currentTarget.src = PlaceholderImage)}
                                    />
                                    <div>
                                        <p className="font-medium">{item.product.productName}</p> {/* Use productName */}
                                        <p className="text-sm text-gray-600">Qty: {item.quantity}</p>
                                    </div>
                                </div>
                                <p className="font-medium">${quantityTotal(item.product.price, item.quantity)}</p>
                            </div>
                        ))}
                        <Separator className="my-4" />
                        <div className="space-y-2">
                            <div className="flex justify-between">
                                <p>Subtotal:</p>
                                <p>${subtotal.toFixed(2)}</p>
                            </div>
                            <div className="flex justify-between">
                                <p>Packing Fee:</p>
                                <p>${PACKING_FEE.toFixed(2)}</p>
                            </div>
                            <div className="flex justify-between">
                                <p>Delivery Fee:</p>
                                <p>${DELIVERY_FEE.toFixed(2)}</p>
                            </div>
                            <div className="flex justify-between">
                                <p>Sales Tax ({SALES_TAX_PERCENT}%):</p>
                                <p>${(subtotal * (SALES_TAX_PERCENT / 100)).toFixed(2)}</p>
                            </div>
                            <Separator className="my-4" />
                            <div className="flex justify-between text-xl font-bold">
                                <p>Total:</p>
                                <p>${calcTotal(subtotal).toFixed(2)}</p>
                            </div>
                        </div>
                    </div>
                    <Button
                        className="w-full mt-6"
                        onClick={() => { handlePlaceOrder(); }}
                        disabled={createOrder.isPending || basket.length === 0} // Disable button while pending or if basket is empty
                    >
                        {createOrder.isPending ? 'Placing Order...' : 'Place Order'}
                    </Button>
                    {createOrder.isError && (
                        <p className="text-red-500 text-sm mt-2">
                            Error placing order: {createOrder.error?.message || "Unknown error"}
                        </p>
                    )}
                </div>
            </div>
        </div>
    );
}