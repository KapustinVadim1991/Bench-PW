const glob = require('glob');
const path = require('path');
const MiniCssExtractPlugin = require('mini-css-extract-plugin');

module.exports = {
    mode: 'development',
    entry: [
        './WebComponents/Assets/Scripts/Test.ts',
        './WebComponents/Assets/Scss/main.scss'
    ],
    output: {
        filename: 'bundle.js',
        path: path.resolve(__dirname, './WebComponents/wwwroot/js'),
        library: '___MyScripts',
        libraryTarget: 'var',
    },
    module: {
        rules: [
            {
                test: /\.s[ac]ss$/i,
                use: [
                    MiniCssExtractPlugin.loader,
                    "css-loader",
                    "sass-loader",
                ],
            },
            {
                test: /\.ts$/,
                use: 'ts-loader',
                exclude: /node_modules/,
            },
            {
                test: /\.m?js$/,
                exclude: /(node_modules|bower_components)/,
                use: {
                    loader: 'babel-loader',
                    options: {
                        presets: ['@babel/preset-env'],
                    },
                },
            },
        ],
    },
    plugins: [
        new MiniCssExtractPlugin({
            filename: '../../wwwroot/css/style.css',
        }),
    ],
};