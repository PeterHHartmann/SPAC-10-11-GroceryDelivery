import { Separator } from '@/components/ui/separator';
import type { Product, ShoppingBasket } from '@/types';
import { useEffect, useMemo, type FC } from 'react';
import PlaceholderImage from '@/assets/placeholder-image.svg';
import { Button } from '@/components/ui/button';
import { Link, useNavigate } from 'react-router-dom';
import { useShoppingBasket } from '@/hooks/shopping-basket';


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
    const subtotal: number = useMemo(() => {
        return calcSubtotal(basket);
    }, [basket]);

    useEffect(() => {
        if (basket.length <= 0) {
            void navigate('/');
        }
    }, [basket, navigate]);

    const handlePlaceOrder = () => {
        void navigate('/order-confirmation'); 
        // wait for 2 seconds before clearing the basket
        setTimeout(() => {
            clearBasket();
        }, 100);
    };

    // Page for payment and delivery details
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
                                <input type="text" name="name" id="name" className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-indigo-500 focus:ring-indigo-500 sm:text-sm py-2" />
                            </div>
                            <div>
                                <label htmlFor="email" className="block text-sm font-medium text-gray-700">Email Address</label>
                                <input type="email" name="email" id="email" className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-indigo-500 focus:ring-indigo-500 sm:text-sm py-2" />
                            </div>
                            <div>
                                <label htmlFor="phone" className="block text-sm font-medium text-gray-700">Phone Number</label>
                                <input type="tel" name="phone" id="phone" className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-indigo-500 focus:ring-indigo-500 sm:text-sm py-2" />
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
                                <input type="text" name="address" id="address" className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-indigo-500 focus:ring-indigo-500 sm:text-sm py-2" />
                            </div>
                            <div>
                                <label htmlFor="city" className="block text-sm font-medium text-gray-700">City</label>
                                <input type="text" name="city" id="city" className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-indigo-500 focus:ring-indigo-500 sm:text-sm py-2" />
                            </div>
                            <div>
                                <label htmlFor="postalCode" className="block text-sm font-medium text-gray-700">Postal Code</label>
                                <input type="text" name="postalCode" id="postalCode" className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-indigo-500 focus:ring-indigo-500 sm:text-sm py-2" />
                            </div>
                            <div>
                                <label htmlFor="country" className="block text-sm font-medium text-gray-700">Country</label>
                                <input type="text" name="country" id="country" className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-indigo-500 focus:ring-indigo-500 sm:text-sm py-2" />
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
                            <div key={item.product.id} className="flex justify-between items-center mb-2">
                                <div className="flex items-center">
                                    <img
                                        src={item.product.imageUrl ?? PlaceholderImage}
                                        alt={item.product.name}
                                        className="w-12 h-12 object-cover rounded mr-3"
                                        onError={(e) => (e.currentTarget.src = PlaceholderImage)}
                                    />
                                    <div>
                                        <p className="font-medium">{item.product.name}</p>
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
                    <Button className="w-full mt-6" onClick={handlePlaceOrder}>
                        Place Order
                    </Button>
                </div>
            </div>
        </div>
    );
}